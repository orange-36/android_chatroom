package com.example.socket;

import androidx.appcompat.app.AppCompatActivity;

import java.io.IOException;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.SocketException;
import java.net.UnknownHostException;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import org.json.JSONException;
import org.json.JSONObject;

public class ClientPage extends AppCompatActivity {
    EditText input_dialog;
    TextView textView;
    Button send_btn;
    Button back_btn;
    private BufferedWriter bw;            //取得網路輸出串流
    private BufferedReader br;            //取得網路輸入串流
    private Thread thread;
    private Socket clientSocket;        //客戶端的socket
    private String tmp;                    //做為接收時的緩存
    private JSONObject json_write, json_read;        //從java伺服器傳遞與接收資

    String ip;
    int port;
    String name;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_client_page);

        input_dialog = findViewById(R.id.editTextTextPersonName3);
        textView = findViewById(R.id.textView5);
        send_btn = findViewById(R.id.button3);
        back_btn = findViewById(R.id.button2);
        send_btn.setOnClickListener(btnlistener);
        Intent it = this.getIntent();
        Bundle bundle = it.getExtras();
        name = bundle.getString("name");
        ip = bundle.getString("ip");
        port = bundle.getInt("port");
        textView.append(name + " connect IP= " + ip + " Port= " + port + "\n");

        thread = new Thread(Connection);                //賦予執行緒工作
        thread.start();                    //讓執行緒開始執行

    }

    private Runnable Connection = new Runnable() {
        @Override
        public void run() {
            // TODO Auto-generated method stub
            try {
                // IP為Server端
                InetAddress serverIp = InetAddress.getByName(ip);
                int serverPort = port;
                clientSocket = new Socket(serverIp, serverPort);
                //取得網路輸出串流
                bw = new BufferedWriter(new OutputStreamWriter(clientSocket.getOutputStream()));
                // 取得網路輸入串流
                br = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
                // 當連線後
                json_write = new JSONObject();
                json_write.put("Uid", "89");
                json_write.put("Username", name);
                json_write.put("Msg", "cat");
                json_write.put("Connect", "1");//傳送離線動作給伺服器
                Log.i("text", "onDestroy()=" + json_write + "\n");
                //寫入後送出
                bw.write(json_write + "\n");
                bw.flush();
                while (clientSocket.isConnected()) {
                    // 取得網路訊息
                    tmp = br.readLine();    //宣告一個緩衝,從br串流讀取值
                    Log.d("text", "thread id=");
                    // 如果不是空訊息
                    if (tmp != null) {
                        //將取到的String抓取{}範圍資料
                        final JSONObject jsonObject = new JSONObject(tmp);
                        final String name = jsonObject.getString("Username");
                        //String title = jsonObject.getString("Username");
                        final String msg = jsonObject.getString("Msg");
                        String info = jsonObject.getString("Connect");
                        if(info.equals("0")){
                            back(back_btn);
                        }
                        tmp = tmp.substring(tmp.indexOf("{"), tmp.lastIndexOf("}") + 1);
                        json_read = new JSONObject(tmp);
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                textView.append(name + ":" + msg + "\n");
                            }
                        });


                        //從java伺服器取得值後做拆解,可使用switch做不同動作的處理
                    }
                }
            } catch (Exception e) {
                //當斷線時會跳到catch,可以在這裡寫上斷開連線後的處理
                Intent it = new Intent();
                it.setClass(ClientPage.this, MainActivity.class);
                startActivity(it);
                e.printStackTrace();
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        Intent it = new Intent();
                        it.setClass(ClientPage.this, MainActivity.class);
                        startActivity(it);
                    }
                });
                Log.e("text", "Socket連線=" + e.toString());
                finish();    //當斷線時自動關閉房間
            }
        }
    };

    public void back(View view) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                json_write = new JSONObject();
                try {
                    json_write.put("Uid", "89");
                    json_write.put("Username", name);
                    json_write.put("Msg", input_dialog.getText());
                    json_write.put("Connect", "0");//傳送離線動作給伺服器
                    // 取得網路輸出串流
                    bw = new BufferedWriter( new OutputStreamWriter(clientSocket.getOutputStream()));

                    // 寫入訊息
                    bw.write(json_write+"\n");
                    bw.flush();
                    // 立即發送

                } catch (JSONException | IOException e) {
                    e.printStackTrace();
                }


            }
        }).start();
        Intent it = new Intent();
        it.setClass(ClientPage.this, MainActivity.class);
        startActivity(it);
    }
    private View.OnClickListener btnlistener = new Button.OnClickListener(){

        @Override
        public void onClick(View v) {
            // TODO Auto-generated method stub
            Thread thread2 = new Thread(Send);                //賦予執行緒工作
            thread2.start();
            input_dialog.setText("");

        }

    };
    private Runnable Send = new Runnable() {
        @Override
        public void run() {
            if(clientSocket.isConnected()){

                json_write = new JSONObject();
                try {
                    json_write.put("Uid", "89");
                    json_write.put("Username", name);
                    json_write.put("Msg", input_dialog.getText());
                    json_write.put("Connect", "1");//傳送離線動作給伺服器
                    // 取得網路輸出串流
                    bw = new BufferedWriter( new OutputStreamWriter(clientSocket.getOutputStream()));
                    // 寫入訊息
                    bw.write(json_write+"\n");
                    bw.flush();
                    // 立即發送
                } catch (JSONException | IOException e) {
                    e.printStackTrace();
                }
                // 將文字方塊清空
                //input_dialog.setText("");
            }
        }
    };

    @Override
    protected void onDestroy() {            //當銷毀該app時
        super.onDestroy();
        try {
            json_write.put("Uid", "89");
            json_write.put("Username", name);
            json_write.put("Msg", input_dialog.getText());
            json_write.put("Connect", "0");//傳送離線動作給伺服器    //傳送離線動作給伺服器
            Log.i("text","onDestroy()="+json_write+"\n");
            //寫入後送出
            bw.write(json_write+"\n");
            bw.flush();
            //關閉輸出入串流後,關閉Socket
            //最近在小作品有發現close()這3個時,導致while (clientSocket.isConnected())這個迴圈內的區域錯誤
            //會跳出java.net.SocketException:Socket is closed錯誤,讓catch內的處理再重複執行,如有同樣問題的可以將下面這3行註解掉
            bw.close();
            br.close();
            clientSocket.close();
        } catch (Exception e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
            Log.e("text","onDestroy()="+e.toString());
        }
    }
}