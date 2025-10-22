using CommunityToolkit.Mvvm.ComponentModel;
using PatternMatchingTool.Data;
using System.IO;

namespace PatternMatchingTool
{
    public class Config
    {
        private string m_strCurrentPath;
        private string m_strRecipePath;
        private string m_strCameraRecipePath;
        private SystemParameter m_objSystemParameter;
        private CameraParameter m_objCameraParameter;
        private RecipeParameter m_objRecipeParameter;
        public bool Initialize()
        {
            bool bReturn = false;

            do
            {
                m_strCurrentPath = Directory.GetCurrentDirectory();

                m_objSystemParameter = new SystemParameter();
                LoadSystemParameter();

                m_strRecipePath = m_objSystemParameter.strRecipePath;
                if (false == Directory.Exists(m_strRecipePath))
                {
                    // 폴더 생성
                    Directory.CreateDirectory(m_strRecipePath);
                }

                m_objCameraParameter = new CameraParameter();
                LoadCameraParameter();

                m_objRecipeParameter = new RecipeParameter();
                LoadRecipeParameter();

                SaveSystemParameter();
                SaveCameraParameter();
                SaveRecipeParameter();
                bReturn = true;
            } while (false);

            return bReturn;
        }

        public void Deinitialize()
        {

        }

        public class SystemParameter : ICloneable
        {

            /// <summary>
            /// Recipe 폴더 위치
            /// </summary>
            public string strRecipePath;

            /// <summary>
            /// Recipe 파일 ID
            /// </summary>
            public string strRecipeID;

            public SystemParameter()
            {
                strRecipePath = Path.Combine(Directory.GetCurrentDirectory(), Define.DEF_RECIPE_PATH);
                strRecipeID = "TEST";
            }

            public object Clone()
            {
                SystemParameter obj = new SystemParameter();
                obj.strRecipePath = this.strRecipePath;
                obj.strRecipeID = this.strRecipeID;

                return obj;
            }
        }

        public SystemParameter GetSystemParameter()
        {
            return m_objSystemParameter.Clone() as SystemParameter;
        }

        public bool LoadSystemParameter()
        {
            bool bReturn = false;

            do
            {
                string strPath = Path.Combine(m_strCurrentPath, Define.DEF_CONFIG_INI);
                ClassINI objINI = new ClassINI(strPath);

                string strSection = "SYSTEM";
                var varParameter = m_objSystemParameter;

                strSection = "RECIPE";
                varParameter.strRecipePath = objINI.GetString(strSection, "strRecipePath", "TEST");
                varParameter.strRecipeID = objINI.GetString(strSection, "strRecipeID", "TEST");

                bReturn = true;
            } while (false);

            return bReturn;
        }

        private bool SaveSystemParameter()
        {
            bool bReturn = false;

            do
            {
                string strPath = Path.Combine(m_strCurrentPath, Define.DEF_CONFIG_INI);
                ClassINI objINI = new ClassINI(strPath);

                string strSection = "SYSTEM";
                var varParameter = m_objSystemParameter;

                strSection = "RECIPE";
                objINI.WriteValue(strSection, "strRecipePath", varParameter.strRecipePath);
                objINI.WriteValue(strSection, "strRecipeID", varParameter.strRecipeID);

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public bool SaveSystemParameter(SystemParameter objSystemParameter)
        {
            bool bReturn = false;

            do
            {
                string strPath = Path.Combine(m_strCurrentPath, Define.DEF_CONFIG_INI);
                ClassINI objINI = new ClassINI(strPath);

                // 원본 대상
                var varParameterOrigin = m_objSystemParameter;
                // 적용 대상
                var varParameter = objSystemParameter;

                string strSection = "SYSTEM";

                strSection = "RECIPE";
                if (varParameterOrigin.strRecipePath != varParameter.strRecipePath)
                    objINI.WriteValue(strSection, "strRecipePath", varParameter.strRecipePath);
                if (varParameterOrigin.strRecipeID != varParameter.strRecipeID)
                    objINI.WriteValue(strSection, "strRecipeID", varParameter.strRecipeID);

                // 원본 바꿔줌
                m_objSystemParameter = objSystemParameter.Clone() as SystemParameter;

            } while (false);

            return bReturn;
        }

        public class CameraParameter : ICloneable
        {
            public int iCameraIP;
            public string strCameraSerialNumber;
            public float fExposureTime;
            public float fGain;
            public float fFrameRate;

            public CameraParameter()
            {
                this.iCameraIP = 0;
                this.strCameraSerialNumber = "";
                this.fExposureTime = 0;
                this.fGain = 0;
                this.fFrameRate = 0;
            }

            public object Clone()
            {
                CameraParameter obj = new CameraParameter();
                obj.iCameraIP = iCameraIP;
                obj.strCameraSerialNumber = this.strCameraSerialNumber;
                obj.fExposureTime = this.fExposureTime;
                obj.fGain = this.fGain;
                obj.fFrameRate = this.fFrameRate;

                return obj;
            }
        }

        public bool LoadCameraParameter()
        {
            bool bReturn = false;
            do
            {
                string strPath = string.Format(@"{0}\{1}", m_strCurrentPath, Define.DEF_CAMERA_INI);
                ClassINI objINI = new ClassINI(strPath);

                string strSection = "CAMERA";
                var varParameter = m_objCameraParameter;
                varParameter.iCameraIP = objINI.GetInt32(strSection, "iCameraIP", 0);
                varParameter.strCameraSerialNumber = objINI.GetString(strSection, "strCameraSerialNumber", "123");
                varParameter.fExposureTime = float.Parse(objINI.GetDouble(strSection, "fExposureTime", 0).ToString());
                varParameter.fGain = float.Parse(objINI.GetDouble(strSection, "fGain", 0).ToString());
                varParameter.fFrameRate = float.Parse(objINI.GetDouble(strSection, "fFrameRate", 0).ToString());

                bReturn = true;
            } while (false);

            return bReturn;
        }

        private bool SaveCameraParameter()
        {
            bool bReturn = false;

            do
            {
                string strPath = string.Format(@"{0}\{1}", m_strCurrentPath, Define.DEF_CAMERA_INI);
                ClassINI objINI = new ClassINI(strPath);

                string strSection = "CAMERA";
                var varParameter = m_objCameraParameter;

                objINI.WriteValue(strSection, "iCameraIP", varParameter.iCameraIP);
                objINI.WriteValue(strSection, "strCameraSerialNumber", varParameter.strCameraSerialNumber);
                objINI.WriteValue(strSection, "fExposureTime", varParameter.fExposureTime);
                objINI.WriteValue(strSection, "fGain", varParameter.fGain);
                objINI.WriteValue(strSection, "fFrameRate", (int)varParameter.fFrameRate);

                bReturn = true;
            } while (false);

            return bReturn;
        }

        /// <summary>
        /// 외부 저장용
        /// </summary>
        /// <param name="objCameraParameter"></param>
        /// <returns></returns>
        public bool SaveCameraParameter(CameraParameter objCameraParameter)
        {
            bool bReturn = false;
            do
            {
                string strPath = string.Format(@"{0}\{1}", m_strCurrentPath, Define.DEF_CAMERA_INI);
                ClassINI objINI = new ClassINI(strPath);

                string strSection = "CAMERA";
                var varParameter = objCameraParameter;

                objINI.WriteValue(strSection, "iCameraIP", varParameter.iCameraIP);
                objINI.WriteValue(strSection, "strCameraSerialNumber", varParameter.strCameraSerialNumber);
                objINI.WriteValue(strSection, "fExposureTime", varParameter.fExposureTime);
                objINI.WriteValue(strSection, "fGain", varParameter.fGain);
                objINI.WriteValue(strSection, "fFrameRate", (int)varParameter.fFrameRate);

                // 원본 바꿔줌
                m_objCameraParameter = objCameraParameter.Clone() as CameraParameter;

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public Config.CameraParameter GetCameraParameter()
        {
            return m_objCameraParameter.Clone() as CameraParameter;
        }

        public class RecipeParameter : ICloneable
        {
            public bool bTryHarder;
            public bool bTryInverted;
            public bool bTryAutoRotae;

            public RecipeParameter()
            {
                bTryHarder = false;
                bTryInverted = false;
                bTryAutoRotae = false;
            }

            public object Clone()
            {
                RecipeParameter obj = new RecipeParameter();
                obj.bTryHarder = this.bTryHarder;
                obj.bTryInverted = this.bTryInverted;
                obj.bTryAutoRotae = this.bTryAutoRotae;

                return obj;
            }
        }

        public bool LoadRecipeParameter()
        {
            bool bReturn = false;
            do
            {
                string strPath = string.Format(@"{0}\{1}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID);
                if (false == Directory.Exists(strPath))
                {
                    // 폴더 생성
                    Directory.CreateDirectory(strPath);
                }
                strPath = string.Format(@"{0}\{1}\{2}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID, Define.DEF_RECIPE_INI);
                ClassINI objINI = new ClassINI(strPath);

                var varParameter = m_objRecipeParameter;
                string strSection = "ID";
                varParameter.bTryHarder = objINI.GetBool(strSection, "bTryHarder", false);
                varParameter.bTryInverted = objINI.GetBool(strSection, "bTryInverted", false);
                varParameter.bTryAutoRotae = objINI.GetBool(strSection, "bTryAutoRotae", false);

                bReturn = true;
            } while (false);

            return bReturn;
        }

        private bool SaveRecipeParameter()
        {
            bool bReturn = false;
            do
            {
                string strPath = string.Format(@"{0}\{1}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID);
                if (false == Directory.Exists(strPath))
                {
                    // 폴더 생성
                    Directory.CreateDirectory(strPath);
                }
                strPath = string.Format(@"{0}\{1}\{2}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID, Define.DEF_RECIPE_INI);
                ClassINI objINI = new ClassINI(strPath);
                var varParameter = m_objRecipeParameter;

                string strSection = "ID";
                objINI.WriteValue(strSection, "bTryHarder", varParameter.bTryHarder);
                objINI.WriteValue(strSection, "bTryInverted", varParameter.bTryInverted);
                objINI.WriteValue(strSection, "bTryAutoRotae", varParameter.bTryAutoRotae);

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public bool SaveRecipeParameter(RecipeParameter objRecipeParameter)
        {
            bool bReturn = false;
            do
            {
                string strPath = string.Format(@"{0}\{1}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID);
                if (false == Directory.Exists(strPath))
                {
                    // 폴더 생성
                    Directory.CreateDirectory(strPath);
                }

                strPath = string.Format(@"{0}\{1}\{2}", m_objSystemParameter.strRecipePath, m_objSystemParameter.strRecipeID, Define.DEF_RECIPE_INI);
                ClassINI objINI = new ClassINI(strPath);
                var varParameter = objRecipeParameter;

                string strSection = "ID";


                objINI.WriteValue(strSection, "bTryHarder", varParameter.bTryHarder);
                objINI.WriteValue(strSection, "bTryInverted", varParameter.bTryInverted);
                objINI.WriteValue(strSection, "bTryAutoRotae", varParameter.bTryAutoRotae);

                // 원본 바꿔줌
                m_objRecipeParameter = objRecipeParameter.Clone() as RecipeParameter;

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public RecipeParameter GetRecipeParameter()
        {
            return m_objRecipeParameter.Clone() as RecipeParameter;
        }
    }
}
