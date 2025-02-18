//=======================================================
// 作者：BrotherChen 
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2020-07-03 17:08:11
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public class udpTest : MonoBehaviour 
	{
        private RadioReport radio;

        private RadioReport radio1;

        private RadioReport radio2;

        private RadioReport radio3;

        private void Start()
        {
            radio = new RadioReport();
            radio.InitUpd(9050);

            radio1 = new RadioReport();
            radio1.InitUpd(9050);

            radio2 = new RadioReport();
            radio2.InitUpd(9050);

            radio3 = new RadioReport();
            radio3.InitUpd(9050);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("fasongxiaoxi sssss");
                radio.RadioToSendOnce(new RadioMsg());
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("fasongxiaoxi dddddd");
                radio1.RadioToSendOnce(new RadioMsg());
            }

            if (Input.GetKeyDown(KeyCode.F))
                {
                Debug.Log("fasongxiaoxi fffff");
                radio2.RadioToSendOnce(new RadioMsg());
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("fasongxiaoxi ggggg");
                radio3.RadioToSendOnce(new RadioMsg());
            }
        }

        private void OnDestroy()
        {
            radio.Close();
            radio1.Close();
            radio2.Close();
            radio3.Close();
        }

        private void OnApplicationQuit()
        {
            radio.Close();
            radio1.Close();
            radio2.Close();
            radio3.Close();
        }
    }
}
