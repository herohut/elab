package eking.tools;

import javax.swing.*;
import java.awt.*;
import java.awt.datatransfer.*;
import java.awt.dnd.*;
//import java.awt.dnd.DragGestureListener;
//import java.awt.dnd.DragSourceListener;
//import java.awt.dnd.DropTargetListener;

import javax.swing.AbstractListModel;
import com.jgoodies.forms.layout.FormLayout;
import com.jgoodies.forms.layout.ColumnSpec;
import com.jgoodies.forms.layout.RowSpec;
import com.jgoodies.forms.factories.FormFactory;
import javax.swing.GroupLayout.Alignment;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.io.File;

public class ComibineOrderControl extends JPanel {

	JList list;
	public ComibineOrderControl() {
		setLayout(new FormLayout(new ColumnSpec[] {
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("default:grow"),
				FormFactory.RELATED_GAP_COLSPEC,
				ColumnSpec.decode("50px"),},
			new RowSpec[] {
				FormFactory.RELATED_GAP_ROWSPEC,
				RowSpec.decode("max(50dlu;default):grow"),}));
		
		list = new JList();
		add(list, "2, 2, fill, fill");
		
		JPanel panel = new JPanel();
		FlowLayout flowLayout = (FlowLayout) panel.getLayout();
		flowLayout.setAlignment(FlowLayout.LEFT);
		add(panel, "4, 2, fill, fill");
		
		JButton btnUp = new JButton("\u02C4");
		btnUp.addMouseListener(new MouseAdapter() {
			@Override
			public void mouseClicked(MouseEvent arg0) {
				move(true);
			}
		});
		panel.add(btnUp);
		
		JButton btnDown = new JButton("\u02C5");
		btnDown.addMouseListener(new MouseAdapter() {
			@Override
			public void mouseClicked(MouseEvent arg0) {
				move(false);
			}
		});
		panel.add(btnDown);
	}
	
	private void move(boolean isUp){
		int index = list.getSelectedIndex();
		if(index==0 && isUp)
			return;
		if(index == _input.length-1 && !isUp)
			return;			
				
		int rIndex = isUp? index-1: index+1;
		File rText = _input[rIndex];
		File val = _input[index];
		_input[rIndex] = val;
		_input[index] = rText;
		setInput(_input);
		list.setSelectedIndex(rIndex);
	}

	private File[] _input;
	public void setInput(File[] input) {
		_input = input;
		list.setModel(new AbstractListModel() {
			File[] values = _input;

			public int getSize() {
				return values.length;
			}

			public Object getElementAt(int index) {
				return values[index].getName();
			}
			
		});

	}

	public File[] getOutput() {
		// TODO Auto-generated method stub
		return _input;
	}	
}
