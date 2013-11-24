package so.libsora.sensorserver;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

public class LogRow {
    String text;
    Date date;
    
    final SimpleDateFormat f = new SimpleDateFormat("[HH:mm:ss]", Locale.KOREA);
    
    public LogRow(String text) {
        this.text = text;
        this.date = new Date();
    }
    
    @Override
    public String toString() {
        return f.format(new Date()) + " " + text;
    }
}
