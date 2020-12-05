package com.example.socket;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioGroup;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {
    EditText name_dialog;
    EditText ip_dialog;
    EditText port_dialog;
    Button send_btn;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        EditText name_dialog = findViewById(R.id.editTextTextPersonName);
        EditText ip_dialog = findViewById(R.id.editTextTextPersonName2);
        EditText port_dialog = findViewById(R.id.editTextTextPersonName4);
        send_btn = findViewById(R.id.button);
    }

    public void send(View view) {
        EditText name_dialog = findViewById(R.id.editTextTextPersonName);
        EditText ip_dialog = findViewById(R.id.editTextTextPersonName2);
        EditText port_dialog = findViewById(R.id.editTextTextPersonName4);

        Bundle bundle = new Bundle();
        if (TextUtils.isEmpty(name_dialog.getText().toString())) {
            name_dialog.setError("Please Enter a username!");
        } else if (TextUtils.isEmpty(ip_dialog.getText().toString())) {
            ip_dialog.setError("Please Enter a ip!");//do something
        } else if (TextUtils.isEmpty(port_dialog.getText().toString())) {
            port_dialog.setError("Please Enter a port!");//do something
        } else {
            bundle.putString("name", name_dialog.getText().toString());
            bundle.putString("ip", ip_dialog.getText().toString());
            bundle.putInt("port", Integer.parseInt(port_dialog.getText().toString()));
            Intent it = new Intent();
            it.putExtras(bundle);
            it.setClass(MainActivity.this, ClientPage.class);
            startActivity(it);
        }
    }
}