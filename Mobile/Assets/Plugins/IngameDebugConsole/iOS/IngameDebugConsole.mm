<<<<<<< HEAD
extern "C" void _DebugConsole_CopyText( const char* text ) 
{
	[UIPasteboard generalPasteboard].string = [NSString stringWithUTF8String:text];
=======
extern "C" void _DebugConsole_CopyText( const char* text ) 
{
	[UIPasteboard generalPasteboard].string = [NSString stringWithUTF8String:text];
>>>>>>> master
}