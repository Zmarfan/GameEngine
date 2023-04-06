using GameEngine.engine.window.menu;
using SDL2;

namespace GameEngine.engine.window; 

public static class WindowMenuHandler {
    public delegate void MenuItemClickedDelegate(string identifier);
    public static event MenuItemClickedDelegate? MenuItemClicked;

    private static nint _mainMenu;
    private static uint _uniqueId = uint.MinValue;
    private static readonly Dictionary<uint, string> UNIQUE_ID_TO_IDENTIFIER = new();
    private static readonly Dictionary<string, uint> IDENTIFIER_TO_UNIQUE_ID = new();
        
    internal static void Init(nint window, WindowMenu menu) {
        nint windowHandle = GetWindowHandle(window);
        SDL.SDL_SetWindowsMessageHook(WindowMessageHook, nint.Zero);

        _mainMenu = WindowApi.CreateMenu();
        CreateMenu(_mainMenu, menu.menuItems);
        WindowApi.SetMenu(windowHandle, _mainMenu);
    }

    public static void ChangeMenuItemCheckStatus(string identifier, bool isChecked) {
        WindowApi.CheckMenuItem(_mainMenu, IDENTIFIER_TO_UNIQUE_ID[identifier], isChecked);
    }
    
    public static void ChangeMenuItemDisabledStatus(string identifier, bool disabled) {
        WindowApi.ChangeMenuItemDisableState(_mainMenu, IDENTIFIER_TO_UNIQUE_ID[identifier], disabled);
    }
    
    private static nint WindowMessageHook(
        nint userdata,
        nint hWnd,
        uint message,
        ulong wParam,
        long lParam
    ) {
        if (message == WindowApi.CLICK_MESSAGE) {
            HandleClickEvent(wParam);
        }
        return WindowApi.CallNextHookEx(nint.Zero, 0, (nint)wParam, (nint)lParam);
    }
    
    private static void HandleClickEvent(ulong menuItemId) {
        MenuItemClicked?.Invoke(UNIQUE_ID_TO_IDENTIFIER[(uint)menuItemId]);
    }

    private static nint GetWindowHandle(nint window) {
        SDL.SDL_VERSION(out SDL.SDL_version version);
        SDL.SDL_SysWMinfo windowInfo = new() { version = version };

        if (SDL.SDL_GetWindowWMInfo(window, ref windowInfo) == SDL.SDL_bool.SDL_FALSE) {
            throw new Exception("Unable to load window handle");
        }

        return windowInfo.info.win.window;
    }

    private static void CreateMenu(nint menu, List<IMenuEntry> entries) {
        foreach (IMenuEntry entry in entries) {
            switch (entry) {
                case MenuItem item:
                    CreateMenuItem(menu, item);
                    break;
                case WindowMenu windowMenu:
                    nint dropdown = WindowApi.CreateMenu();
                    WindowApi.AppendMenu(menu, WindowApi.MENU_FLAG_POPUP, (uint)dropdown, windowMenu.text);
                    CreateMenu(dropdown, windowMenu.menuItems);
                    break;
                case MenuBreak:
                    WindowApi.AppendMenu(menu, WindowApi.MENU_FLAG_SEPARATOR);
                    break;
            }
        }
    }

    private static void CreateMenuItem(nint menu, MenuItem item) {
        uint flags = WindowApi.MENU_FLAG_STRING;
        flags |= item.disabled ? WindowApi.MENU_FLAG_GRAYED : 0;
        flags |= item.isChecked ? WindowApi.MENU_FLAG_CHECKED : 0;
        uint uniqueId = _uniqueId++;
        UNIQUE_ID_TO_IDENTIFIER.Add(uniqueId, item.identifier);
        IDENTIFIER_TO_UNIQUE_ID.Add(item.identifier, uniqueId);
        string text = item.rightText == null ? item.text : $"{item.text}\t{item.rightText}";
        
        WindowApi.AppendMenu(menu, flags, uniqueId, text);
    }
}