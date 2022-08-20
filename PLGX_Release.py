import tkinter
import os
import subprocess
import zipfile
import io
import requests
import tempfile
import Cryptodome
import Cryptodome.Hash
import Cryptodome.Hash.SHA512
import Cryptodome.Signature
import Cryptodome.Signature.pkcs1_15
import Cryptodome.PublicKey
import Cryptodome.PublicKey.RSA
import base64

class SelectTag(tkinter.Tk):
	def __init__(self, choices, master=None):	
		tkinter.Tk.__init__(self, master)
		self.choices = choices
		self.geometry('180x200')   
		self.title('Select tag to release')
		
		# Create a listbox
		self.listbox = tkinter.Listbox(self, width=40, height=10)
		self.listbox.pack()
		
		self.btn_ok = tkinter.Button(self, text='Release', command=self.Ok)
		self.btn_ok.pack(side='right')
		
		self.btn_cancel = tkinter.Button(self, text='Cancel', command=self.destroy)
		self.btn_cancel.pack(side='left')
	
		# Inserting the listbox items
		index = 0
		for choice in self.choices:
			self.listbox.insert(index, choice[1])
			index += 1
			
		self.ok = False
			
	def Ok(self):
		selection = self.listbox.curselection()
		assert(len(selection) == 1)
		self.ok = True
		self.result = selection[0]
		self.destroy()
		
def remove_prefix(prefix, text):
    assert( text.startswith(prefix) )
    return text[len(prefix):]
	
def PackageFiles(keepass, files):
	extensions = [".cs", ".resx", ".csproj"]

	with tempfile.TemporaryDirectory(prefix="KeeLocker_") as tempdir:
		print( "Packaging in {}".format(tempdir))
			
		projpath = ""
		for file in files:
			filename = file[0]
			filebytes = file [1]
			ext = os.path.splitext(filename)[1]
			if not ext in extensions:
				continue
				
			(reldir, f) = os.path.split(filename)
			dirpath = os.path.join(tempdir, reldir)
			filepath = os.path.join(tempdir, filename)
			if ext == ".csproj":
				projpath = os.path.split(filepath)[0]
				
			os.makedirs( dirpath, exist_ok=True )
				
			with open(filepath, "wb") as out:
				out.write(filebytes) 
	
		plgx_proc = subprocess.run([keepass, "--plgx-create", projpath, "--plgx-prereq-os:Windows"], cwd=tempdir)
		
		LocalDir = os.path.dirname(__file__)
		PackagePath = os.path.join(LocalDir, "KeeLocker.plgx")
		with open( os.path.join(tempdir, "KeeLocker.plgx"), "rb") as src:
			with open(PackagePath, "wb") as dst:
				dst.write(src.read())
		
def SignRelease(Tag):
	print( "Signing {}".format(Tag))
	VersionInfo = b"KeeLocker:" + Tag.encode("utf-8") + b"\n"
	LocalDir = os.path.dirname(__file__)
	PrivateKeyPath = os.path.join(LocalDir, "KeeLocker_private_signing_key.p8")
	VersionInfoPath = os.path.join(LocalDir, "VersionInfo.txt")
	
	Hasher = Cryptodome.Hash.SHA512.new()
	Hasher.update(VersionInfo)
	
	Separator = b":"
	
	with open(PrivateKeyPath, "rb") as KeyFile:
		PrivateKey = Cryptodome.PublicKey.RSA.import_key(KeyFile.read());
		Signer = Cryptodome.Signature.pkcs1_15.new(PrivateKey)
		Signature = Signer.sign(Hasher)
		Signatureb64 = base64.b64encode(Signature)
		with open(VersionInfoPath, "wb") as VersionInfoFile:
			VersionInfoFile.write(Separator)
			VersionInfoFile.write(Signatureb64)
			VersionInfoFile.write(b"\n")
			VersionInfoFile.write(VersionInfo)
			VersionInfoFile.write(Separator)
			VersionInfoFile.write(b"\n")
	

def DoRelease(selected_tag):
	print( "Releasing {}{}".format(selected_tag[0], selected_tag[1]))
	
	response = requests.get("https://api.github.com/repos/Gugli/KeeLocker/zipball/{}".format(selected_tag[0]))
	if(response.status_code != 200):
		print( "Unable to fetch code" )	
		return
		
	archive_bytes = io.BytesIO(response.content)
	files = []
	with zipfile.ZipFile(archive_bytes) as archive:	
		folders = []
		for info in archive.infolist():
			if info.is_dir():
				folders.append( info.filename )

		root = min(folders, key=len)
		
		print( "Archive root {}".format(root) )
		
		for info in archive.infolist():		
			if info.is_dir():
				continue
			prefname = remove_prefix(root, info.filename)
			cleanname = os.path.normpath( prefname )
			with archive.open(info) as file:
				files.append( ( cleanname, file.read()) )
			
	PackageFiles("C:\\Users\\Gugli\\KeePassDEV\\KeePass.exe", files)
		
		
		
if __name__ == '__main__':
	git_lstags = subprocess.run(["git", "ls-remote", "--tags", "git@github.com:Gugli/KeeLocker.git"], stdout=subprocess.PIPE)
	resultstr = git_lstags.stdout.decode("utf-8")
	tags = []
	for line in resultstr.split("\n"):
		if len(line) == 0: continue
		infos = line.split()
		tags.append( infos )
	
	wnd = SelectTag(tags)  
	wnd.mainloop() 
	if(wnd.ok):
		selected_tag = tags[wnd.result]
		DoRelease(selected_tag)
		SignRelease(remove_prefix("refs/tags/v", selected_tag[1]))
		os.system("pause") 