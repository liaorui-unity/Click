package com.unity3d.player;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.graphics.PixelFormat;
import android.graphics.Point;
import android.os.Build;
import android.os.IBinder;
import android.provider.Settings;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.WindowManager;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import org.jetbrains.annotations.Nullable;

import com.unity3d.player.UnityPlayer;


public class UnityService extends Service{
    //要引用的布局文件.
    LinearLayout touchLayout;
    //布局参数
    WindowManager.LayoutParams params;
    //实例化的WindowManager.
    WindowManager windowManager;

    protected UnityPlayer mUnityPlayer;

    @Override
    public void onCreate() {
        super.onCreate();
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private void createToucher(UnityPlayer mup) {
        //赋值WindowManager&LayoutParam.
        params = new WindowManager.LayoutParams();
        windowManager = (WindowManager) getApplication().getSystemService(Context.WINDOW_SERVICE);
        //根据版本设置type
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            params.type = WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY;
        } else {
            params.type = WindowManager.LayoutParams.TYPE_PHONE;
        }
        //设置效果为背景透明.
        params.format = PixelFormat.RGBA_8888;
        //设置透明度
        params.alpha = 1.0f;
        //设置flags.不可聚焦及不可使用按钮对悬浮窗进行操控.
        params.flags = WindowManager.LayoutParams.FLAG_NOT_TOUCH_MODAL | WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE;

        //设置窗口初始停靠位置.
        params.gravity = Gravity.END | Gravity.BOTTOM;
        params.x = 0;
        params.y = 0;


        Point point = new Point();
        windowManager.getDefaultDisplay().getRealSize(point);
        DisplayMetrics dm = new DisplayMetrics();
        windowManager.getDefaultDisplay().getMetrics(dm);
        int dpi = (int) (dm.density*160);

        //设置悬浮窗口长宽数据.
        params.width = 200*(dpi/160);
        params.height = 300*(dpi/160);

        LayoutInflater inflater = LayoutInflater.from(getApplication());
        //获取浮动窗口视图所在布局.
        touchLayout = (LinearLayout) inflater.inflate(R.layout.layout,null);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (Settings.canDrawOverlays(this)) {
                //添加toucherlayout
                windowManager.addView(touchLayout, params);
            }
        }


        mUnityPlayer = mup;
        ((RelativeLayout) touchLayout.findViewById(R.id.rl_pet)).addView(mUnityPlayer);
        //mUnityPlayer.start();
        mUnityPlayer.resume();

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {

        UnityPlayer upa = MUnityActivity.mup;
        createToucher(upa);
        mUnityPlayer.windowFocusChanged(true);

        return super.onStartCommand(intent, flags, startId);
    }

    // Quit Unity
    @Override public void onDestroy ()
    {
        mUnityPlayer.pause();
        //mUnityPlayer.stop();
        mUnityPlayer.quit();
        super.onDestroy();
    }
}
