
## 中文

cefglue崩溃问题


复现步骤

1. 点击Create Account大量创建cefglue
2. 右键uuid按钮大量关闭cefglue
3. 重复步骤1和2，直到崩溃

### error信息

```
CoreCLR Version: 9.0.625.26613
.NET Version: 9.0.6
Description: The process was terminated due to an unhandled exception.
Stack:
   at Xilium.CefGlue.Interop.cef_preference_manager_t.release(Xilium.CefGlue.Interop.cef_preference_manager_t*)
   at Xilium.CefGlue.CefPreferenceManager.Release()
   at Xilium.CefGlue.CefPreferenceManager.Finalize()
   at System.GC.RunFinalizers()

```

## English

Reproduction Steps
1. Click "Create Account" to create a large number of cefglue instances.
2. Right-click the UUID button to close a large number of cefglue instances.
3. Repeat steps 1 and 2 until a crash occurs.

### error message

```
CoreCLR Version: 9.0.625.26613
.NET Version: 9.0.6
Description: The process was terminated due to an unhandled exception.
Stack:
   at Xilium.CefGlue.Interop.cef_preference_manager_t.release(Xilium.CefGlue.Interop.cef_preference_manager_t*)
   at Xilium.CefGlue.CefPreferenceManager.Release()
   at Xilium.CefGlue.CefPreferenceManager.Finalize()
   at System.GC.RunFinalizers()

```