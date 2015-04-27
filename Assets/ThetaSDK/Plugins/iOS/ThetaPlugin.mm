//
//  Theta.mm
//  Unity-iPhone
//
//  Created by noshipu on 2015/04/15.
//
//
#if UNITY_VERSION <= 434
mport "iPhone_View.h"
#endif

// Theta API
#import "PtpConnection.h"
#import "PtpLogging.h"

extern "C" void UnitySendMessage(const char *, const char *, const char *);

inline static void dispatch_async_main(dispatch_block_t block)
{
    dispatch_async(dispatch_get_main_queue(), block);
}

// define unity callback type
typedef void (*ConnectSuccess)();
typedef void (*ConnectError)();
typedef void (*DisConnectSuccess)();
typedef void (*DisConnectError)();
typedef void (*CaptureSuccess)();
typedef void (*CaptureError)();

@interface ThetaPlugin : UIViewController<PtpIpEventListener>
{
    PtpConnection* _ptpConnection;
    NSString *gameObjectName;
    
    int imageWidth;
    int imageHeight;
}
@end

@implementation ThetaPlugin

- (id)initWithGameObjectName:(const char *)game_object_name
{
    self = [super init];
    
    // init ptp
    _ptpConnection = [[PtpConnection alloc] init];
    [_ptpConnection setLoglevel:PTPIP_LOGLEVEL_WARN];
    [_ptpConnection setEventListener:self];
    
    gameObjectName = [NSString stringWithUTF8String:game_object_name];
    
    return self;
}

#pragma mark - PtpIpEventListener delegates.
- (void)ptpip_eventReceived:(int)code :(uint32_t)param1 :(uint32_t)param2 :(uint32_t)param3 {
    switch (code) {
        default:
        {
            dispatch_async_main(^{
                NSLog(@"don`t define callback");
            });
        }
            break;
            
        case PTPIP_OBJECT_ADDED:
        {
            [_ptpConnection operateSession:^(PtpIpSession *session) {
                // param1 in handle
                [self loadObject:param1 session:session];
                dispatch_async_main(^{
                    NSLog(@"call back add");
                });
            }];
        }
            break;
            
    }
}

-(void)ptpip_socketError:(int)err
{
    // If libptpip closed the socket, `closed` is non-zero.
    BOOL closed = PTP_CONNECTION_CLOSED(err);
    
    // PTPIP_PROTOCOL_*** or POSIX error number (errno()).
    err = PTP_ORIGINAL_PTPIPERROR(err);
    
    NSArray* errTexts = @[@"Socket closed",              // PTPIP_PROTOCOL_SOCKET_CLOSED
                          @"Brocken packet",             // PTPIP_PROTOCOL_BROCKEN_PACKET
                          @"Rejected",                   // PTPIP_PROTOCOL_REJECTED
                          @"Invalid session id",         // PTPIP_PROTOCOL_INVALID_SESSION_ID
                          @"Invalid transaction id.",    // PTPIP_PROTOCOL_INVALID_TRANSACTION_ID
                          @"Unrecognided command",       // PTPIP_PROTOCOL_UNRECOGNIZED_COMMAND
                          @"Invalid receive state",      // PTPIP_PROTOCOL_INVALID_RECEIVE_STATE
                          @"Invalid data length",        // PTPIP_PROTOCOL_INVALID_DATA_LENGTH
                          @"Watchdog expired",           // PTPIP_PROTOCOL_WATCHDOG_EXPIRED
                          ];
    NSString* desc;
    if ((PTPIP_PROTOCOL_SOCKET_CLOSED<=err) && (err<=PTPIP_PROTOCOL_WATCHDOG_EXPIRED)) {
        desc = [errTexts objectAtIndex:err-PTPIP_PROTOCOL_SOCKET_CLOSED];
    } else {
        desc = [NSString stringWithUTF8String:strerror(err)];
    }
    dispatch_async_main(^{
        UnitySendMessage([gameObjectName UTF8String],
                         "CallbackError", [desc UTF8String]);
    });
}

- (void)connect:(const char *)ip_address success:(ConnectSuccess)success error:(ConnectError)error
{
    NSString *_ip_address = [NSString stringWithUTF8String:ip_address ? ip_address : ""];
    [_ptpConnection setTargetIp:_ip_address];
    [_ptpConnection connect:^(BOOL connected){
        if(connected) {
            dispatch_async_main(^{
                success();
            });
        }
        else {
            dispatch_async_main(^{
                error();
            });
        }
    }];
}

- (void)disConnect:(DisConnectSuccess)success error:(DisConnectError)error
{
    [_ptpConnection close:^{
        dispatch_async_main(^{
            success();
        });
    }];
}

- (void)capture:(CaptureSuccess)success error:(CaptureError)error
{
    // session open
    [_ptpConnection operateSession:^(PtpIpSession *session) {
        // capture
        BOOL rtn = [session initiateCapture];
        dispatch_async_main(^{
            if(rtn) {
                success();
            }else{
                error();
            }
        });
    }];
}

- (void)loadObject:(uint32_t)objectHandle session:(PtpIpSession*)session
{
    // get object informations.
    PtpIpObjectInfo* objectInfo = [session getObjectInfo:objectHandle];
    if (!objectInfo) {
        return;
    }
    if (objectInfo.object_format != PTPIP_FORMAT_JPEG) {
        return;
    }
    
    // load 2048x1024 image
    imageWidth = 2048;
    imageHeight = 1024;
    NSMutableData* image_data = [NSMutableData data];
    BOOL result = [session getResizedImageObject:objectHandle
                                           width:imageWidth
                                          height:imageHeight
                                     onStartData:^(NSUInteger totalLength) {
                                         // Callback reception.
                                         NSLog(@"getThumb(0x%08x) will received %zd bytes.", objectHandle, totalLength);
                                     }
                                 onChunkReceived:^BOOL(NSData *data) {
                                     [image_data appendData:data];
                                     return YES;
                                 }];
    
    if (!result) {
        // failed
        return;
    }
    
    dispatch_async_main(^{
        // save image
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSString *documentsDirectory = [paths objectAtIndex:0];
        NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"tmp_pic.jpg"];
        [image_data writeToFile:filePath atomically:YES];
        
        // call unity method
        UnitySendMessage([gameObjectName UTF8String],
                         "CallbackObjectAdd", MakeStringCopy([filePath UTF8String]));
    });
}

char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}
@end

extern "C" {
    void *Theta_Connect(const char *ip_address, const char *game_object_name,
                        ConnectSuccess success, ConnectError error);
    void Theta_DisConnect(void *instance, DisConnectSuccess success, DisConnectError error);
    void Theta_Capture(void *instance, CaptureSuccess success, CaptureError error);
}

void *Theta_Connect(const char *ip_address, const char *game_object_name,
                    ConnectSuccess success, ConnectError error)
{
    id instance = [[ThetaPlugin alloc] initWithGameObjectName:game_object_name];
    
    // connection
    [instance connect:ip_address success:success error:error];
    return (__bridge void *)(instance);
}

void Theta_DisConnect(void* instance, DisConnectSuccess success, DisConnectError error) {
    ThetaPlugin *thetaPlugin = (__bridge ThetaPlugin *)(instance);
    [thetaPlugin disConnect:success error:error];
}

void Theta_Capture(void* instance, CaptureSuccess success, CaptureError error) {
    ThetaPlugin *thetaPlugin = (__bridge ThetaPlugin *)(instance);
    [thetaPlugin capture:success error:error];
}