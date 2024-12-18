﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeeLocker
{
	public class FveApi
	{

		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 584)]
		internal struct FVE_AUTH_ELEMENT
		{
			[FieldOffset(0)]
			public Int32 MagicValue;

			[FieldOffset(4)]
			public Int32 MustBeOne;

			[FieldOffset(8)]
			public byte Data_Start;
		}

		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		internal struct FVE_UNLOCK_SETTINGS
		{
			[FieldOffset(0x00)]
			public Int32 rsp_30;

			[FieldOffset(0x04)]
			public Int32 rsp_34;

			[FieldOffset(0x08)]
			public Int32 rsp_38;

			[FieldOffset(0x0C)]
			public Int32 rsp_3C;

			[FieldOffset(0x10)]
			public IntPtr rsp_40; // FVE_AUTH_ELEMENT**

			[FieldOffset(0x18)]
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
		internal static extern HRESULT FveAuthElementFromPassPhrase(string PassPhrase, ref FVE_AUTH_ELEMENT AuthElement);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveAuthElementFromRecoveryPassword")]
		internal static extern HRESULT FveAuthElementFromRecoveryPassword(string PassPhrase, ref FVE_AUTH_ELEMENT AuthElement);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveOpenVolume")]
		internal static extern HRESULT FveOpenVolume(string VolumeId, Int32 FlagsMaybe, ref IntPtr HVolume);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveUnlockVolumeWithAccessMode")]
		internal static extern HRESULT FveUnlockVolumeWithAccessMode(IntPtr HVolume, ref FVE_UNLOCK_SETTINGS UnlockSettings, Int32 FlagsMaybe);
		[DllImport("FVEAPI.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FveCloseVolume")]
		internal static extern HRESULT FveCloseVolume(IntPtr HVolume, ref FVE_UNLOCK_SETTINGS UnlockSettings, Int32 FlagsMaybe, Int32 PassPhrase);

		public enum Result
		{
			Ok,
			Unexpected,
			DriveNotFound,
			WrongPassPhrase,
		}

		private static IntPtr StructToPointer(object Struct)
		{
			IntPtr Pointer = Marshal.AllocHGlobal(Marshal.SizeOf(Struct));
			Marshal.StructureToPtr(Struct, Pointer, false);
			return Pointer;
		}

		public static bool GetDriveGUID(string DriveMountPoint, out string DriveGUID)
		{
			const int MaxVolumeNameLength = 50;
			StringBuilder DriveGUIDWriter = new StringBuilder(MaxVolumeNameLength);

			bool Ok = GetVolumeNameForVolumeMountPoint(DriveMountPoint, DriveGUIDWriter, (uint)DriveGUIDWriter.MaxCapacity);
			if (!Ok)
			{
				DriveGUID = "";
				return false;
			}
			DriveGUID = DriveGUIDWriter.ToString();
			return true;
		}

		public static Result UnlockVolume(string DriveMountPoint, string DriveGUID, string PassPhrase, bool IsRecoveryKey)
		{
			Result R = Result.Ok;

			IntPtr pAuthElement = (IntPtr)0;
			IntPtr ppAuthElement = (IntPtr)0;
			IntPtr pUnlockSettings = (IntPtr)0;
			do
			{
				if (DriveGUID.Length == 0)
				{
					bool Ok = GetDriveGUID(DriveMountPoint, out DriveGUID);
					if (!Ok)
					{
						R = Result.DriveNotFound;
						break;
					}
				}

				HRESULT HResult;

				FVE_AUTH_ELEMENT AuthElement = new FVE_AUTH_ELEMENT();
				Int32 SecretType;
				if (IsRecoveryKey)
				{
					SecretType = (Int32)FVE_SECRET_TYPE.RecoveryPassword;

					AuthElement.MagicValue = 32;
					AuthElement.MustBeOne = 1;
					HResult = FveAuthElementFromRecoveryPassword(PassPhrase, ref AuthElement);
					if (HResult != 0)
					{
						R = Result.WrongPassPhrase;
						break;
					}

				}
				else
				{
					SecretType = (Int32)FVE_SECRET_TYPE.PassPhrase;
					AuthElement.MagicValue = 578;
					AuthElement.MustBeOne = 1;
					HResult = FveAuthElementFromPassPhrase(PassPhrase, ref AuthElement);
					if (HResult != 0)
					{
						R = Result.WrongPassPhrase;
						break;
					}
				}

				IntPtr HVolume = (IntPtr)0;
				HResult = FveOpenVolume(DriveGUID, 0, ref HVolume);
				if (HResult != 0)
				{
					R = Result.Unexpected;
					break;
				}


				pAuthElement = StructToPointer(AuthElement);
				ppAuthElement = StructToPointer(pAuthElement);

				FVE_UNLOCK_SETTINGS UnlockSettings = new FVE_UNLOCK_SETTINGS();
				UnlockSettings.rsp_30 = 56;
				UnlockSettings.rsp_34 = 1;
				UnlockSettings.rsp_38 = SecretType;
				UnlockSettings.rsp_3C = 1;
				UnlockSettings.rsp_40 = ppAuthElement;
				UnlockSettings.rsp_48 = 0;

				Int32 FlagsMaybe = 0;
				HResult = FveUnlockVolumeWithAccessMode(HVolume, ref UnlockSettings, FlagsMaybe);
				if (HResult == HRESULT.FVE_E_FAILED_AUTHENTICATION)
				{
					R = Result.WrongPassPhrase;
					break;
				}

				if (HResult != 0)
				{
					R = Result.Unexpected;
					break;
				}

				HResult = FveCloseVolume(HVolume, ref UnlockSettings, FlagsMaybe, SecretType);
				if (HResult != 0)
				{
					R = Result.Unexpected;
					break;
				}

				// Free the unmanaged memory.

			} while (false);

			if (ppAuthElement != (IntPtr)0)
				Marshal.FreeHGlobal(ppAuthElement);

			if (pAuthElement != (IntPtr)0)
				Marshal.FreeHGlobal(pAuthElement);

			return R;

		}

        internal enum NTSTATUS
        {
            S_OK = unchecked((int)0x00000000),
        }

		internal const ulong WNF_FVE_STATE_CHANGE = 0x4183182BA3BC3875UL;


        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RtlSubscribeWnfStateChangeNotification")]
        internal static extern NTSTATUS RtlSubscribeWnfStateChangeNotification(out IntPtr Subscription, ulong StateName, int ChangeStamp, IntPtr Callback, IntPtr CallbackContext, IntPtr TypeId, int SerializationGroup, int Unknown);
        [DllImport("ntdll.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RtlUnsubscribeWnfStateChangeNotification")]
        internal static extern NTSTATUS RtlUnsubscribeWnfStateChangeNotification(IntPtr Subscription);

        public delegate void TOnStateChangeDelegate();

        public delegate int TCallbackDelegate(
                    ulong StateName,
                    int ChangeStamp,
                    IntPtr TypeId,
                    IntPtr CallbackContext,
                    IntPtr Buffer,
                    int BufferSize);

		internal struct SCallbackContext
		{
            public TOnStateChangeDelegate Callback;
        };

        public struct SSubscription
		{
			public TOnStateChangeDelegate OnStateChange; // Have to keep that to prevent GC
            public IntPtr InternalPointer;
			public TCallbackDelegate Callback;
			public IntPtr CallbackContextPtr;

            public void Clear() {
                InternalPointer = IntPtr.Zero;
            }
            public bool IsValid()
            {
                return InternalPointer != IntPtr.Zero;
            }
        };



        static private int OnWnfStateChange(
            ulong stateName,
            int nChangeStamp,
            IntPtr pTypeId,
            IntPtr pCallbackContext,
            IntPtr pBuffer,
            int nBufferSize)
        {
			if (stateName != WNF_FVE_STATE_CHANGE) 
				return 0;

			if (pCallbackContext == IntPtr.Zero) return 1;

            SCallbackContext CallbackContext =  Marshal.PtrToStructure<SCallbackContext>(pCallbackContext);
			CallbackContext.Callback();
            return 0;
        }

        public static SSubscription StateChangeNotification_Subscribe(TOnStateChangeDelegate _OnStateChange  )
		{
            TCallbackDelegate Callback = new TCallbackDelegate(OnWnfStateChange);

            SCallbackContext CallbackContext = new SCallbackContext();
            CallbackContext.Callback = _OnStateChange;

            IntPtr CallbackContextPtr = Marshal.AllocHGlobal(Marshal.SizeOf(CallbackContext));
            Marshal.StructureToPtr(CallbackContext, CallbackContextPtr, false);
           
            IntPtr pSubscription = IntPtr.Zero;
            NTSTATUS ntstatus = RtlSubscribeWnfStateChangeNotification(
                out pSubscription,
                WNF_FVE_STATE_CHANGE,
                0,
                Marshal.GetFunctionPointerForDelegate(Callback),
                CallbackContextPtr,
                IntPtr.Zero,
                0,
                0);

            SSubscription R;
			R.OnStateChange = _OnStateChange;
            R.InternalPointer = pSubscription;
            R.Callback = Callback;
            R.CallbackContextPtr = CallbackContextPtr;
            return R;
		}

		public static void StateChangeNotification_Unsubscribe(SSubscription Subscription)
        {
            if (Subscription.InternalPointer != IntPtr.Zero)
            {
                RtlUnsubscribeWnfStateChangeNotification(Subscription.InternalPointer);
                Marshal.FreeHGlobal(Subscription.CallbackContextPtr);
            }
        }
	}
}
