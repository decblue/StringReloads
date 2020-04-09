﻿namespace StringReloads.Hook
{
    public unsafe class MultiByteToWideChar : Base.Hook<MultiByteToWideCharDelegate>
    {
        public override string Library => "Kernel32.dll";

        public override string Export => "MultiByteToWideChar";

        public override void Initialize()
        {
            HookDelegate = new MultiByteToWideCharDelegate(hMultiByteToWideChar);
            Compile();
        }

        private int hMultiByteToWideChar(uint CodePage, uint dwFlags, byte* lpMultiByteStr, int cbMultiByte, ushort* lpWideCharStr, int cchWideChar)
        {
            int Rst = 0;
            if (cbMultiByte > 0)
            {
                byte[] Buffer = new byte[cbMultiByte];
                for (int i = 0; i < Buffer.Length; i++)
                {
                    Buffer[i] = *(lpMultiByteStr + i);
                }
                fixed (void* pBuffer = &Buffer[0])
                {
                    Uninstall();
                    lpMultiByteStr = (byte*)EntryPoint.Process(pBuffer);
                    Install();
                    Rst = Bypass(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, lpWideCharStr, cchWideChar);
                }

            } else {
                Uninstall();
                lpMultiByteStr = (byte*)EntryPoint.Process((void*)lpMultiByteStr);
                Install();
                Rst = Bypass(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, lpWideCharStr, cchWideChar);
            }
            return Rst;
        }
    }
}
