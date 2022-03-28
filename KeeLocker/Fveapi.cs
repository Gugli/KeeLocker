using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KeeLocker
{
	public class FveApi
	{

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
		unsafe internal struct FVE_AUTH_ELEMENT
		{
			public Int32 MagicValue;
			public Int32 MustBeOne;
			public fixed byte Data[576];
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
		unsafe internal struct FVE_UNLOCK_SETTINGS
		{
			public Int32 rsp_30;
			public Int32 rsp_34;
			public Int32 rsp_38;
			public Int32 rsp_3C;
			public FVE_AUTH_ELEMENT** rsp_40;
			public Int64 rsp_48;
		};


		internal enum HRESULT
		{
			S_OK = unchecked((int)0x00000000),
			FVE_E_FAILED_AUTHENTICATION = unchecked((int)0x80310027),
		}
		internal enum FVE_SECRET_TYPE
		{
			PassPhrase = unchecked((int)0x800000),
			RecoveryPassword = unchecked((int)0x80000),
		}

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeNameForVolumeMountPointW")]
		internal static extern bool GetVolumeNameForVolumeMountPoint(string lpszVolumeMountPoint, [Out] StringBuilder lpszVolumeName, uint cchBufferLength);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveAuthElementFromPassPhrase")]
		internal unsafe static extern HRESULT FveAuthElementFromPassPhrase(string PassPhrase, FVE_AUTH_ELEMENT* AuthElement);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveOpenVolume")]
		internal unsafe static extern HRESULT FveOpenVolume(string VolumeId, Int32 FlagsMaybe, IntPtr* HVolume);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveUnlockVolumeWithAccessMode")]
		internal unsafe static extern HRESULT FveUnlockVolumeWithAccessMode(IntPtr HVolume, FVE_UNLOCK_SETTINGS* UnlockSettings, Int32 FlagsMaybe);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveCloseVolume")]
		internal unsafe static extern HRESULT FveCloseVolume(IntPtr HVolume, FVE_UNLOCK_SETTINGS* UnlockSettings, Int32 FlagsMaybe, Int32 PassPhrase);

		public enum Result
		{
			Ok,
			Unexpected,
			DriveNotFound,
			WrongPassPhrase,
		}

		public unsafe static Result UnlockVolume(string DriveMountPoint, string PassPhrase)
		{
			FVE_AUTH_ELEMENT AuthElement;
			AuthElement.MagicValue = 578;
			AuthElement.MustBeOne = 1;

			HRESULT HResult;

			HResult = FveAuthElementFromPassPhrase(PassPhrase, &AuthElement);
			if (HResult != 0)
				return Result.WrongPassPhrase;

			const int MaxVolumeNameLength = 50;
			StringBuilder VolumeId = new StringBuilder(MaxVolumeNameLength);

			bool Ok = GetVolumeNameForVolumeMountPoint(DriveMountPoint, VolumeId, (uint)MaxVolumeNameLength);

			if (!Ok)
				return Result.DriveNotFound;

			IntPtr HVolume;
			HResult = FveOpenVolume(VolumeId.ToString(), 0, &HVolume);
			if (HResult != 0)
				return Result.Unexpected;

			FVE_AUTH_ELEMENT* pAuthElement = &AuthElement;

			FVE_UNLOCK_SETTINGS UnlockSettings;
			UnlockSettings.rsp_30 = 56;
			UnlockSettings.rsp_34 = 1;
			UnlockSettings.rsp_38 = (Int32)FVE_SECRET_TYPE.PassPhrase;
			UnlockSettings.rsp_3C = 1;
			UnlockSettings.rsp_40 = &pAuthElement;
			UnlockSettings.rsp_48 = 0;

			Int32 FlagsMaybe = 0;
			HResult = FveUnlockVolumeWithAccessMode(HVolume, &UnlockSettings, FlagsMaybe);
			if (HResult == HRESULT.FVE_E_FAILED_AUTHENTICATION )
				return Result.WrongPassPhrase;

			if (HResult != 0)
				return Result.Unexpected;

			HResult = FveCloseVolume(HVolume, &UnlockSettings, FlagsMaybe, (Int32)FVE_SECRET_TYPE.PassPhrase);
			if (HResult != 0)
				return Result.Unexpected;

			return Result.Ok;
		}
	}

}
