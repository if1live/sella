package so.libsora.sensorserver;

import java.util.ArrayList;

import android.content.Context;
import android.widget.ArrayAdapter;
import android.widget.ListView;

public class LogView {
    public static final int MAX_LOG = 512;
    
    ListView listView;
    
    ArrayList<LogRow> arrayList;
    ArrayAdapter<LogRow> adapter;
    
    public LogView(Context context, ListView listView) {
        this.listView = listView;
        
        arrayList = new ArrayList<LogRow>();
        adapter = new ArrayAdapter<LogRow>(context, android.R.layout.simple_list_item_1, arrayList);
        listView.setAdapter(adapter);
    }
    
    /**
     * 새로 추가한 행이 제일 위에 오도록한다
     * log 최대 갯수는 일정범위로 유지한다
     * @param row
     */
    public void add(LogRow row) {
        if(adapter.getCount() > MAX_LOG) {
            int lastIndex = adapter.getCount() - 1;
            LogRow last = adapter.getItem(lastIndex);
            adapter.remove(last);
        }
        adapter.insert(row, 0);
    }
}
