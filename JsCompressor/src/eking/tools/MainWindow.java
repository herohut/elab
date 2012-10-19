package eking.tools;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.EventQueue;

import javax.swing.JFrame;
import java.awt.GridLayout;

import javax.swing.JComponent;
import javax.swing.JDialog;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JPasswordField;
import javax.swing.JTextField;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import java.awt.event.ActionListener;
import java.awt.event.ActionEvent;
import javax.swing.JTextPane;

import org.eclipse.swt.internal.ole.win32.VARDESC;
import org.mozilla.javascript.ErrorReporter;
import org.mozilla.javascript.EvaluatorException;

import com.yahoo.platform.yui.compressor.JavaScriptCompressor;

import java.awt.Toolkit;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.FilenameFilter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.Reader;
import java.io.Writer;
import javax.swing.JTabbedPane;
import java.awt.GridBagLayout;

public class MainWindow {

	private JFrame frmJscompressor;
	private JCheckBox chckbxBackup;
	private JCheckBox chckbxObfuscation;
	private JTextPane txtMinifyLog;
	private FileBrowser minifyFileBrowser;
	private JCheckBox chkCombine;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					MainWindow window = new MainWindow();
					window.frmJscompressor.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	/**
	 * Create the application.
	 */
	public MainWindow() {
		initialize();
	}

	/**
	 * Initialize the contents of the frame.
	 */
	private void initialize() {
		frmJscompressor = new JFrame();
		frmJscompressor.setIconImage(Toolkit.getDefaultToolkit().getImage(
				MainWindow.class.getResource("/eking/Logo-64px.png")));
		frmJscompressor.setTitle("JsCompressor");
		frmJscompressor.setResizable(false);
		frmJscompressor.setBounds(100, 100, 606, 339);
		frmJscompressor.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frmJscompressor.getContentPane().setLayout(null);

		JButton btnAction = new JButton("DO IT");
		btnAction.setBounds(10, 284, 151, 26);
		frmJscompressor.getContentPane().add(btnAction);

		txtMinifyLog = new JTextPane();
		txtMinifyLog.setBounds(10, 72, 578, 201);
		frmJscompressor.getContentPane().add(txtMinifyLog);
		txtMinifyLog.setEditable(false);

		chckbxBackup = new JCheckBox("Backup");
		chckbxBackup.setBounds(10, 40, 74, 23);
		frmJscompressor.getContentPane().add(chckbxBackup);
		chckbxBackup.setSelected(true);

		chckbxObfuscation = new JCheckBox("Obfuscation");
		chckbxObfuscation.setBounds(100, 40, 106, 23);
		frmJscompressor.getContentPane().add(chckbxObfuscation);
		chckbxObfuscation.setSelected(true);

		JCheckBox chckbxJavascript = new JCheckBox("Javascript");
		chckbxJavascript.setBounds(210, 40, 98, 23);
		frmJscompressor.getContentPane().add(chckbxJavascript);
		chckbxJavascript.setSelected(true);

		JCheckBox chckbxCss = new JCheckBox("CSS");
		chckbxCss.setBounds(310, 40, 74, 23);
		frmJscompressor.getContentPane().add(chckbxCss);
		chckbxCss.setSelected(true);

		JCheckBox chckbxTree = new JCheckBox("Tree");
		chckbxTree.setBounds(410, 40, 66, 23);
		frmJscompressor.getContentPane().add(chckbxTree);

		minifyFileBrowser = new FileBrowser();
		minifyFileBrowser.setBounds(10, 10, 578, 23);
		frmJscompressor.getContentPane().add(minifyFileBrowser);

		chkCombine = new JCheckBox("Combine");
		chkCombine.setBounds(510, 40, 78, 23);
		frmJscompressor.getContentPane().add(chkCombine);
		btnAction.addMouseListener(new MouseAdapter() {
			@Override
			public void mouseClicked(MouseEvent arg0) {
				try {
					Action(minifyFileBrowser.getTxtLocationText());
					JOptionPane.showMessageDialog(frmJscompressor, "Success");
				} catch (Exception ex) {
					JOptionPane.showMessageDialog(frmJscompressor,
							ex.getMessage(), "Error", JOptionPane.ERROR_MESSAGE);
				}
			}
		});
	}

	private void Action(String input) throws Exception {
		File f = new File(input);
		if (f.isFile()) {
			handleCompress(f);
			return;
		}
		File[] files = f.listFiles(new FilenameFilter() {
			@Override
			public boolean accept(File arg0, String arg1) {
				return arg1.endsWith(".js");
			}
		});

		File full = null;
		if (chkCombine.isSelected()) {
			ComibineOrderControl combine = new ComibineOrderControl();
			combine.setInput(files);
			int option = JOptionPane.showConfirmDialog(frmJscompressor,
					combine, "Order", JOptionPane.OK_CANCEL_OPTION);
			if (option != 0)
				return;
			files = combine.getOutput();

			File master = files[files.length - 1];
			String fName = getFileNameWithoutExtension(master);
			String fullFPath = master.getParent() + "\\" + fName + ".Full.js";
			full = new File(fullFPath);
			StringBuilder sBuilder = new StringBuilder();
			for (File file : files) {
				String fText = getContents(file);
				sBuilder.append(fText);
			}

			String fullText = sBuilder.toString();
			setContents(full, fullText);
		}
		if (full != null) {
			handleCompress(full);
			if (!chckbxBackup.isSelected())
				return;

			for (File file : files) {
				file.delete();
			}

			return;
		}
		for (File file : files) {
			backup(file);
			handleCompress(file);
		}
	}

	private void backup(File file) throws IOException {
		String fDir = file.getParent();
		String backupdir = fDir + "\\backup\\";

		File f = new File(backupdir);
		if ((!f.exists() || !f.isDirectory()))
			f.mkdir();

		String path = file.getAbsolutePath();

		String cloneFile = backupdir + getFileNameWithoutExtension(file)
				+ ".bak.js";
		copy(path, cloneFile);

	}

	static public String getContents(File aFile) throws Exception {
		// ...checks on aFile are elided
		StringBuilder contents = new StringBuilder();

		try {
			// use buffering, reading one line at a time
			// FileReader always assumes default encoding is OK!
			InputStreamReader reader = new InputStreamReader(
					new FileInputStream(aFile), charset);

			BufferedReader input = new BufferedReader(reader);
			try {
				String line = null; // not declared within while loop
				/*
				 * readLine is a bit quirky : it returns the content of a line
				 * MINUS the newline. it returns null only for the END of the
				 * stream. it returns an empty String if two newlines appear in
				 * a row.
				 */
				while ((line = input.readLine()) != null) {
					contents.append(line);
					contents.append(System.getProperty("line.separator"));
				}
			} finally {
				input.close();
			}
		} catch (IOException ex) {
			ex.printStackTrace();
		}

		return contents.toString();
	}

	/**
	 * Change the contents of text file in its entirety, overwriting any
	 * existing text.
	 * 
	 * This style of implementation throws all exceptions to the caller.
	 * 
	 * @param aFile
	 *            is an existing file which can be written to.
	 * @throws IllegalArgumentException
	 *             if param does not comply.
	 * @throws FileNotFoundException
	 *             if the file does not exist.
	 * @throws IOException
	 *             if problem encountered during write.
	 */
	static public void setContents(File aFile, String aContents)
			throws FileNotFoundException, IOException {

		// use buffering

		OutputStreamWriter writer = new OutputStreamWriter(
				new FileOutputStream(aFile), charset);
		Writer output = new BufferedWriter(writer);
		try {
			// FileWriter always assumes default encoding is OK!
			output.write(aContents);
		} finally {
			output.close();
		}
	}

	public static void copy(String fromFileName, String toFileName)
			throws IOException {
		File fromFile = new File(fromFileName);
		File toFile = new File(toFileName);

		FileInputStream from = null;
		FileOutputStream to = null;
		try {
			from = new FileInputStream(fromFile);
			to = new FileOutputStream(toFile);
			byte[] buffer = new byte[4096];
			int bytesRead;

			while ((bytesRead = from.read(buffer)) != -1)
				to.write(buffer, 0, bytesRead); // write
		} finally {
			if (from != null)
				try {
					from.close();
				} catch (IOException e) {
					;
				}
			if (to != null)
				try {
					to.close();
				} catch (IOException e) {
					;
				}
		}
	}

	private static String charset = "UTF-8";

	private void compressFile(String input, String output) throws Exception {

		Reader reader = null;
		Writer writer = null;
		try {
			reader = new InputStreamReader(new FileInputStream(input), charset);

			JavaScriptCompressor compressor = new JavaScriptCompressor(reader,
					new ErrorReporter() {

						@Override
						public void warning(String arg0, String arg1, int arg2,
								String arg3, int arg4) {
						}

						@Override
						public EvaluatorException runtimeError(String arg0,
								String arg1, int arg2, String arg3, int arg4) {
							return null;
						}

						@Override
						public void error(String arg0, String arg1, int arg2,
								String arg3, int arg4) {

						}
					});

			writer = new OutputStreamWriter(new FileOutputStream(output),
					charset);

			compressor.compress(writer, -1, true, true, true,
					chckbxObfuscation.isSelected());

		} catch (Exception ex) {
			throw ex;

		} finally {
			try {
				reader.close();
				writer.close();
			} catch (Exception ex) {

			}
		}
	}

	private void handleCompress(File file) throws Exception {
		txtMinifyLog.setText(txtMinifyLog.getText() + "\n"
				+ file.getAbsoluteFile());

		String fDir = file.getParent();		

		String path = file.getAbsolutePath();

		String cloneFile = fDir + "\\" + getFileNameWithoutExtension(file)
				+ ".clone";
		copy(path, cloneFile);
		
		file.delete();		

		compressFile(cloneFile, path);

		if (!chckbxBackup.isSelected())
			(new File(cloneFile)).delete();
	}

	private String getFileNameWithoutExtension(File file) {
		String fName = file.getName();
		return fName.substring(0, fName.lastIndexOf("."));
	}
}
