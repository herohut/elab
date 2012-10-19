package eking.tools;

import javax.swing.JFileChooser;
import javax.swing.JPanel;
import javax.swing.JButton;

import java.awt.Component;
import java.awt.GridBagLayout;
import java.awt.GridBagConstraints;
import javax.swing.JTextField;
import java.awt.Insets;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.io.File;

public class FileBrowser extends JPanel {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private JTextField txtLocation;

	/**
	 * Create the panel.
	 */
	public FileBrowser() {
		final Component me = this;
		GridBagLayout gridBagLayout = new GridBagLayout();
		gridBagLayout.columnWidths = new int[]{179, 100, 0};
		gridBagLayout.rowHeights = new int[]{23, 0};
		gridBagLayout.columnWeights = new double[]{1.0, 0.0, Double.MIN_VALUE};
		gridBagLayout.rowWeights = new double[]{1.0, Double.MIN_VALUE};
		setLayout(gridBagLayout);
		
		txtLocation = new JTextField();
		GridBagConstraints gbc_txtLocation = new GridBagConstraints();
		gbc_txtLocation.insets = new Insets(0, 0, 0, 5);
		gbc_txtLocation.fill = GridBagConstraints.BOTH;
		gbc_txtLocation.gridx = 0;
		gbc_txtLocation.gridy = 0;
		add(txtLocation, gbc_txtLocation);
		txtLocation.setColumns(10);
		
		JButton btnBrowse = new JButton("Browse...");
		btnBrowse.addMouseListener(new MouseAdapter() {			
			@Override
			public void mouseClicked(MouseEvent arg0) {

				JFileChooser fileChooser = new JFileChooser();
				fileChooser
						.setFileSelectionMode(JFileChooser.FILES_AND_DIRECTORIES);
				fileChooser.showDialog(me, "OK");
				fileChooser.setMultiSelectionEnabled(false);
				File file = fileChooser.getSelectedFile();
				if (file == null)
					return;
				txtLocation.setText(file.getAbsolutePath());
			}
		});
		GridBagConstraints gbc_btnBrowse = new GridBagConstraints();
		gbc_btnBrowse.fill = GridBagConstraints.BOTH;
		gbc_btnBrowse.gridx = 1;
		gbc_btnBrowse.gridy = 0;
		add(btnBrowse, gbc_btnBrowse);
	}
	

	public String getTxtLocationText() {
		return txtLocation.getText();
	}
	public void setTxtLocationText(String text) {
		txtLocation.setText(text);
	}
}
