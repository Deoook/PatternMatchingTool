using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvCamCtrl.NET;
using PatternMatchingTool.Data;
using PatternMatchingTool.Device.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatchingTool.Process
{
    public class ProcessCameraManager
    {
        public DeviceCamera m_objCamera;
        public MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList;
        public ProcessCameraManager()
        {
            
        }

        public bool Initialize()
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;
            do
            {
                m_objCamera = new DeviceCamera(new DeviceCameraHikvision() as DeviceCameraAbstract);

                //연결된 카메라 List 찾기
                SearchDevice();

                // 시작할 때 카메라 열어두기
                m_objCamera.OpenDevice(pDocument.m_objConfig.GetCameraParameter());

                bReturn = true;
            } while (false);

            return bReturn;
        }

        public void Deinitialize()
        {
            m_objCamera.CloseDevice();
        }
        public List<string> SearchDevice()
        {
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                Console.WriteLine("Search Device Fail");
            }

            List<string> deviceList = new List<string>();
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                string strUserDefinedName = "";

                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if ((gigeInfo.chUserDefinedName.Length > 0) && (gigeInfo.chUserDefinedName[0] != '\0'))
                    {
                        //if (MyCamera.IsTextUTF8(gigeInfo.chUserDefinedName))
                        //{
                        //    strUserDefinedName = Encoding.UTF8.GetString(gigeInfo.chUserDefinedName).TrimEnd('\0');
                        //}
                        //else
                        //{
                        //    strUserDefinedName = Encoding.Default.GetString(gigeInfo.chUserDefinedName).TrimEnd('\0');
                        //}
                        deviceList.Add("GEV: " + strUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        deviceList.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
            }

            return deviceList;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
