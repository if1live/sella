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
     * ���� �߰��� ���� ���� ���� �������Ѵ�
     * log �ִ� ������ ���������� �����Ѵ�
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
