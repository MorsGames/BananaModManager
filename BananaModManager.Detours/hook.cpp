#include "pch.h"

#include <Windows.h>
#include "detours.h"

extern "C"
{
	__declspec(dllexport) void hook_attach(void** from, void* to)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(from, to);
		
		const auto error = DetourTransactionCommit();
		if (error != NO_ERROR) {
			MessageBox(HWND_DESKTOP, L"Failed to detour!", L"BananaModManager", MB_OK);
		}
	}

	__declspec(dllexport) void hook_detach(void** from, void* to)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(from, to);

		const auto error = DetourTransactionCommit();
		if (error != NO_ERROR) {
			MessageBox(HWND_DESKTOP, L"Failed to detach!", L"BananaModManager", MB_OK);
		}
	}
}