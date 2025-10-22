using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Data
{
    public class Define
    {
        public const string DEF_CONFIG_INI = "Config.ini";
        public const string DEF_RECIPE_PATH = "VisionRecipe";
        public const string DEF_RECIPE_INI = "VisionRecipe.ini";
        public const string DEF_CAMERA_INI = "VisionCamera.ini";

        /// <summary>
        /// 검사할 패턴 타입
        /// </summary>
        public enum PatternType
        {
            PATTERN_TYPE_ID = 0, PATTERN_TYPE_PATTERN_MATCHING
        };

        /// <summary>
        /// 유저가 선택한 Run Mode
        /// </summary>
        public enum RunMode
        {
            RUN_MODE_IDLE = 0, RUN_MODE_RUNNING, RUN_MODE_STOP 
        };

        /// <summary>
        /// 통신으로 받아온 Trigger
        /// </summary>
        public enum Trigger
        {
            TRIGGER_ON, TRIGGER_ID, TRIGGER_PATTERN_MATCHING, TRIGGER_OFF
        };
    }
}
