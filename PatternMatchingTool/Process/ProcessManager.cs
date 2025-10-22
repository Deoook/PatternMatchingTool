using PatternMatchingTool.Data;
using System.Xml.Linq;

namespace PatternMatchingTool.Process
{
    //검사 순서 관리 클래스
    public class ProcessManager
    {
        private System.Windows.Forms.Timer m_objProcessTimer;

        public ProcessManager()
        {

        }

        public bool Initialize()
        {
            var pDocument = Document.GetDocument;
            bool bReturn = false;
            do
            {


                //타이머 설정
                m_objProcessTimer = new System.Windows.Forms.Timer();
                m_objProcessTimer.Interval = 500;
                m_objProcessTimer.Tick += timer_Elapsed;
                m_objProcessTimer.Start();

                bReturn = true;
            } while (false);
            return bReturn;
        }

        // 타이머 말고 Thread로 구현?
        private void timer_Elapsed(object sender, EventArgs e)
        {
            var pDocument = Document.GetDocument;

            // 검사 대기중이 아니면 Return
            if (Define.RunMode.RUN_MODE_IDLE == pDocument.GetRunMode())
                DoProcess();

        }

        public void Deinitialize()
        {
            var pDocument = Document.GetDocument;
            //카메라 콜백 해제
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnProcessFrameReceived -= OnProcessFrameReceived;
        }

        private void DoProcess()
        {
            var pDocument = Document.GetDocument;

            do
            {
                // 현재 실행 중이면 Break.
                if (Define.RunMode.RUN_MODE_RUNNING == pDocument.GetRunMode())
                    break;

                // Camera 연결 안했으면 연결
                if (false == pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.IsConnected())
                {
                    if (false == pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OpenDevice(pDocument.m_objConfig.GetCameraParameter()))
                        break;
                }

                // 트리거 Off 상태면 Break.
                if (Define.Trigger.TRIGGER_OFF == pDocument.GetTrigger())
                    break;

                //카메라 콜백 연결
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnProcessFrameReceived += OnProcessFrameReceived;

                // Grab Start
                if (false == pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.StartGrab())
                {
                    pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.StopGrab();
                    break;
                }

                pDocument.SetRunMode(Define.RunMode.RUN_MODE_RUNNING);

                // 트리거 하나 날림
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();
            } while (false);
        }

        private void OnProcessFrameReceived(Bitmap bmp)
        {
            //Trigger 확인해서
            //ID인지 Pattern인지 확인 후에
            //검사하고
            //결과 Bitmap위에 그린다음에 다시 뿌리기...
            var pDocument = Document.GetDocument;

            if (Define.Trigger.TRIGGER_ON == pDocument.GetTrigger())
            {
                //검사 트리거가 아니면 그냥 뿌리고 끝
                pDocument.AutoVM.DisplayBitmap = bmp;
            }
            else if (Define.Trigger.TRIGGER_ID == pDocument.GetTrigger())
            {
                //ID 검사
            }
            else if (Define.Trigger.TRIGGER_PATTERN_MATCHING == pDocument.GetTrigger())
            {
                //패턴 매칭 검사
            }

            // Grab 중지
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.StopGrab();

            // Trigger Off
            pDocument.SetTrigger(Define.Trigger.TRIGGER_OFF);
            pDocument.SetRunMode(Define.RunMode.RUN_MODE_IDLE);
        }
    }
}
