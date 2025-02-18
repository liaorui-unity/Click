package com.unity3d.player;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.Settings;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

    public class MUnityActivity extends UnityPlayerActivity {
        public static UnityPlayer mup;
        @Override
        protected void onCreate(Bundle savedInstanceState) {
            super.onCreate(savedInstanceState);

            if(mup == null)
            {
                mup = mUnityPlayer;
            }

            if (!Settings.canDrawOverlays(this))
            {
                Toast.makeText(this, "当前无权限，请授权", Toast.LENGTH_SHORT).show();
                startActivityForResult(new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION, Uri.parse("package:" + getPackageName())), 1);
            }
            else {
                Toast.makeText(this, "启动！！！", Toast.LENGTH_SHORT).show();
                startService(new Intent(MUnityActivity.this, UnityService.class));
                moveTaskToBack(true);
            }
        }

        @Override
        protected void onActivityResult(int requestCode, int resultCode, Intent data) {
            if (requestCode == 1) {
                if (!Settings.canDrawOverlays(this))
                {
                    Toast.makeText(this, "授权失败", Toast.LENGTH_SHORT).show();
                } else
                {
                    Toast.makeText(this, "授权成功", Toast.LENGTH_SHORT).show();
                    startService(new Intent(MUnityActivity.this, UnityService.class));
                    moveTaskToBack(true);
                }
            }
        }
    }

