// Unityのバージョンが4.3.4以前であれば
#if UNITY_VERSION <= 434

// Unity4.3.4ではUnityGetGLViewControllerを使用するためにiPhone_Viewのインポートが必要
#import "iPhone_View.h"
#endif


// APIライブラリ
#import "PtpConnection.h"
#import "PtpLogging.h"

// EXIFパースライブラリ
#import "RicohEXIF.h"
#import "ExifTags.h"


extern "C" {
    inline static void dispatch_async_main(dispatch_block_t block)
    {
        dispatch_async(dispatch_get_main_queue(), block);
    }
    
    PtpConnection* _ptpConnection;
    
    
    void Theta_Connect() {
        _ptpConnection = [[PtpConnection alloc] init];
        [_ptpConnection setTargetIp:@"192.168.1.1"];
        [_ptpConnection connect:^(BOOL connected){
        }];
    }
    
    void Theta_DisConnect() {
        [_ptpConnection close:^{}];
    }
    
    void Theta_Capture() {
        [_ptpConnection operateSession:^(PtpIpSession *session)
         {
             // This block is running at PtpConnection#gcd thread.
             BOOL rtn = [session initiateCapture];
             dispatch_async_main(^{
             });
         }];
    }
}