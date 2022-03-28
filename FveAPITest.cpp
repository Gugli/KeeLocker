#include <iostream>

#include <windows.h>

enum ESecretType : uint32_t{
    PassPhrase             = 0x800000,
    RecoveryPassword       = 0x80000,
};

union FVE_AUTH_ELEMENT
{
    unsigned char AsBytes[584];

    struct {
        int32_t MagicValue;     // 0
        int32_t MustBeOne;      // 4 
    } S;
};

struct UNLOCK_SETTINGS {
    int32_t     rsp_30;       
    int32_t     rsp_34;       
    int32_t     rsp_38;       
    int32_t     rsp_3C;       
    void*       rsp_40;       
    void*       rsp_48;       
    //void*       rsp_50;       
    //void*       rsp_58;        
    //void*       rsp_60;       
    //void*       rsp_68;     
};

typedef HRESULT(__stdcall*TFveAuthElementFromPassPhraseW)(const wchar_t* PassPhrase, const FVE_AUTH_ELEMENT* AuthElement);
typedef HRESULT(__stdcall*TFveAuthElementFromRecoveryPasswordW)(const wchar_t* RecoveryPassword, const FVE_AUTH_ELEMENT* AuthElement);
typedef HRESULT(__stdcall*TFveOpenVolumeW)(const wchar_t* VolumeId, int64_t FlagsMaybe, HANDLE* Volume);
typedef HRESULT(__stdcall*TFveUnlockVolumeWithAccessMode)(HANDLE Volume, UNLOCK_SETTINGS* UnlockSettings, uint32_t FlagsMaybe);

typedef HRESULT(__stdcall*TFveCloseVolume)(HANDLE Volume, UNLOCK_SETTINGS* UnlockSettings, uint32_t FlagsMaybe, ESecretType SecretType);



HRESULT UnlockDrive(const wchar_t* VolumeId, const wchar_t* PassphraseOrRecoveryPassword, ESecretType SecretType, uint32_t FlagsMaybe)
{
    HINSTANCE dllHandle = LoadLibrary(L"fveapi.dll");
    
    if(!dllHandle)
        return E_UNEXPECTED;

    TFveAuthElementFromPassPhraseW FveAuthElementFromPassPhraseW = (TFveAuthElementFromPassPhraseW) ::GetProcAddress(dllHandle, "FveAuthElementFromPassPhraseW");
    TFveAuthElementFromRecoveryPasswordW FveAuthElementFromRecoveryPasswordW = (TFveAuthElementFromRecoveryPasswordW) ::GetProcAddress(dllHandle, "FveAuthElementFromRecoveryPasswordW");
    TFveOpenVolumeW FveOpenVolumeW = (TFveOpenVolumeW) ::GetProcAddress(dllHandle, "FveOpenVolumeW");
    TFveCloseVolume FveCloseVolume = (TFveCloseVolume) ::GetProcAddress(dllHandle, "FveCloseVolume");
    TFveUnlockVolumeWithAccessMode FveUnlockVolumeWithAccessMode = (TFveUnlockVolumeWithAccessMode) ::GetProcAddress(dllHandle, "FveUnlockVolumeWithAccessMode");

    FVE_AUTH_ELEMENT AuthElement;
    memset(&AuthElement, 0, sizeof(FVE_AUTH_ELEMENT));
    HRESULT Result;
    if(SecretType == ESecretType::PassPhrase) {
        AuthElement.S.MagicValue = 578;
        AuthElement.S.MustBeOne = 1;
        Result = FveAuthElementFromPassPhraseW(PassphraseOrRecoveryPassword, &AuthElement);
    } else if (SecretType == ESecretType::RecoveryPassword) {     
        AuthElement.S.MagicValue = 32;
        AuthElement.S.MustBeOne = 1;
        Result = FveAuthElementFromRecoveryPasswordW(PassphraseOrRecoveryPassword, &AuthElement);
    }
    if(Result != S_OK)
        return Result;

    HANDLE HVolume;
    Result = FveOpenVolumeW(VolumeId, 0, &HVolume);

    if(Result != S_OK)
        return Result;

    FVE_AUTH_ELEMENT* pAuthElement = &AuthElement;

    UNLOCK_SETTINGS UnlockSettings;
    UnlockSettings.rsp_30 = 56;
    UnlockSettings.rsp_34 = 1;
    UnlockSettings.rsp_38 = SecretType;
    UnlockSettings.rsp_3C = 1;
    UnlockSettings.rsp_40 = &pAuthElement;
    UnlockSettings.rsp_48 = 0;
    Result = FveUnlockVolumeWithAccessMode(HVolume, &UnlockSettings, FlagsMaybe);

    if(Result != S_OK)
        return Result;
    
    Result = FveCloseVolume(HVolume, &UnlockSettings, FlagsMaybe, SecretType);

    if(Result != S_OK)
        return Result;

    FreeLibrary(dllHandle);  

    return S_OK;
}


int main()
{

    const wchar_t* Passphrase = L"SECRET";
    const wchar_t* VolumeMuntPoint = L"H:\\";
    ESecretType SecretType = ESecretType::PassPhrase;
    uint32_t FlagsMaybe = 0;

    wchar_t VolumeId[50];
    bool r = ::GetVolumeNameForVolumeMountPointW(VolumeMuntPoint, VolumeId, 50 );

    HRESULT Result = UnlockDrive(VolumeId, Passphrase, SecretType, FlagsMaybe);

    return Result;
}
