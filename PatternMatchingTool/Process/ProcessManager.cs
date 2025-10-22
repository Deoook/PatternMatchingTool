using PatternMatchingTool.Data;
using System.Xml.Linq;

namespace PatternMatchingTool.Process
{
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
                // 카메라 콜백 연결
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnProcessFrameReceived += OnProcessFrameReceived;

                // 타이머 설정
                m_objProcessTimer = new System.Windows.Forms.Timer();
                m_objProcessTimer.Interval = 100;
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

            // RUN_MODE_IDLE 일 경우 검사 준비 시작
            if (Define.RunMode.RUN_MODE_IDLE == pDocument.GetRunMode())
                pDocument.m_objProcessMain.m_objProcessCameraManager.Start();
            // RUN_MODE_READY이고 TRIGGER가 켜졌으면 검사 시작
            else if (Define.RunMode.RUN_MODE_READY == pDocument.GetRunMode() && Define.Trigger.TRIGGER_OFF != pDocument.GetTrigger())
                DoProcess();
            // RUN_MODE가 STOP이면 해제, Trigger OFF 처리
            else if (Define.RunMode.RUN_MODE_STOP == pDocument.GetRunMode())
            {
                pDocument.m_objProcessMain.m_objProcessCameraManager.Stop();
                pDocument.SetTrigger(Define.Trigger.TRIGGER_OFF);
            }
        }

        public void Deinitialize()
        {
            var pDocument = Document.GetDocument;
            // 카메라 콜백 해제
            pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.OnProcessFrameReceived -= OnProcessFrameReceived;
        }

        private void DoProcess()
        {
            var pDocument = Document.GetDocument;

            do
            {
                // 동작 중으로 상태 바꾸고
                pDocument.SetRunMode(Define.RunMode.RUN_MODE_RUNNING);

                // 트리거 하나 날림
                pDocument.m_objProcessMain.m_objProcessCameraManager.m_objCamera.Trigger();
            } while (false);
        }

        private void OnProcessFrameReceived(Bitmap bmp)
        {
            // Trigger 확인해서
            // ID인지 Pattern인지 확인 후에
            // 검사하고
            // 결과 Bitmap위에 그린다음에 뿌리기...
            var pDocument = Document.GetDocument;

            if (Define.Trigger.TRIGGER_ON == pDocument.GetTrigger())
            {
                // 검사 트리거가 아니면 그냥 뿌리고 끝
                pDocument.AutoVM.DisplayBitmap = bmp;
            }
            else if (Define.Trigger.TRIGGER_ID == pDocument.GetTrigger())
            {
                // ID 검사
                // ProcessID Class 따로 구현 후에 Return 받기?
            }
            else if (Define.Trigger.TRIGGER_PATTERN_MATCHING == pDocument.GetTrigger())
            {
                // 패턴 매칭 검사
            }

            // Trigger Off
            pDocument.SetTrigger(Define.Trigger.TRIGGER_OFF);
            
            // RunMode 다시 Ready 상태로 변경
            pDocument.SetRunMode(Define.RunMode.RUN_MODE_READY);
        }
    }
}
