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
using System.Xml.Linq;

namespace PatternMatchingTool.Process
{
    public class ProcessCameraManager
    {
        public DeviceCamera m_objCamera;
        public MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList;
        public bool m_bisStart = false;
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
                //m_objCamera.OpenDevice(pDocument.m_objConfig.GetCameraParameter());

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

        public bool Start()
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;

            do
            {
                // 이미 시작 중일 경우
                if(true == m_bisStart)
                {
                    bReturn = true;
                    break;
                }

                // 카메라 열고
                if (false == m_objCamera.OpenDevice(pDocument.m_objConfig.GetCameraParameter()))
                    break;

                // 카메라 설정 넣어주고
                m_objCamera.SetExposureTime(pDocument.m_objConfig.GetCameraParameter().fExposureTime);
                m_objCamera.SetGain(pDocument.m_objConfig.GetCameraParameter().fGain);
                m_objCamera.SetFrameRate(pDocument.m_objConfig.GetCameraParameter().fFrameRate);

                // Grab Start
                if (false == m_objCamera.StartGrab())
                    break;

                // 검사 준비 완료 상태로 변경
                pDocument.SetRunMode(Define.RunMode.RUN_MODE_READY);

                m_bisStart = true;
                bReturn = true;
            } while (false);

            return bReturn;
        }

        public void Stop()
        {
            // 시작 중이 아닐 경우
            if (false == m_bisStart)
                return;

            var pDocument = Document.GetDocument;

            // Grab Stop
            m_objCamera.StopGrab();
            // 카메라 닫기
            m_objCamera.CloseDevice();

            // 정지 상태로 변경
            pDocument.SetRunMode(Define.RunMode.RUN_MODE_STOP);

            m_bisStart = false;
        }
    }
}
