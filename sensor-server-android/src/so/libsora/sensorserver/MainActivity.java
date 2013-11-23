package so.libsora.sensorserver;

import android.hardware.SensorManager;
import android.os.Bundle;
import android.app.Activity;
import android.content.Context;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class MainActivity extends Activity {

    private SensorManager sensorManager;
    private TextView textView;
    private OrientationSeneor orientationSensor;
    private SensorUpdater updater;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView = (TextView)findViewById(R.id.sensorTextView);

        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        orientationSensor = new OrientationSeneor(sensorManager);
        
        updater = new SensorUpdater(orientationSensor, policy);
        
        Button resetButton = (Button)findViewById(R.id.resetButton);
        Button startButton = (Button)findViewById(R.id.startButton);
        Button stopButton = (Button)findViewById(R.id.stopButton);
        
        resetButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                orientationSensor.reset();
            }
        });
        startButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                orientationSensor.registerListener();
                updater.start();
            }
        });
        stopButton.setOnClickListener(new OnClickListener() {            
            @Override
            public void onClick(View v) {
                orientationSensor.unregisterListener();
                updater.stop();
            }
        });
        
        //http://stackoverflow.com/questions/13406180/how-to-get-local-ip-and-display-it-in-textview
        NetworkHelper networkHelper = new NetworkHelper(this);
        TextView ipTextView = (TextView)findViewById(R.id.ipTextView);
        ipTextView.setText(networkHelper.getIP());
        
        SensorTCPServer server = new SensorTCPServer(orientationSensor);
        server.start();
    }

    @Override
    protected void onResume() {
        super.onResume();
        orientationSensor.registerListener();
        updater.start();
    }

    @Override
    protected void onPause() {
        super.onPause();
        orientationSensor.unregisterListener();
        updater.stop();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    SensorUpdaterPolicy policy = new SensorUpdaterPolicy() {
        @Override
        public void run(float yaw, float pitch, float roll) {
            String log = String.format("%.3f, %.3f, %.3f", yaw, pitch, roll);
            textView.setText(log);   
        }
    };
}
