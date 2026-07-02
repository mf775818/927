namespace ShoeMoldControl.Core
{
    public class DecodeResult
    {
        public bool IsSuccess { get; set; }
        public string DecodedText { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public enum RobotMode
    {
        INIT = 1, 
        BRAKE_OPEN = 2, 
        POWEROFF = 3, 
        DISABLED = 4, 
        ENABLE = 5, 
        BACKDRIVE = 6, 
        RUNNING = 7, 
        ERROR = 9
    }
}
