using System;
using System.ComponentModel.DataAnnotations;

namespace RetroDb.Data
{
    [Flags]
    public enum GameControlType
    {
        None = 0,
        [Display(Name = "Game pad")]
        GamePad = 1 << 0,
        Keyboard = 1 << 1,
        Joystick = 1 << 2,
        Mouse = 1 << 3,
        [Display(Name = "Virtual Reality")]
        VR = 1 << 4,
    }

    public enum GameSystemType
    {
        None = 0,
        Arcade = 1 << 0,
        Computer = 1 << 1,
        Console = 1 << 2,
        Handheld = 1 << 3,
        Pinball = 1 << 4
    }
}
