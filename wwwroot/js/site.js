const dropArea = document.getElementById('dropContainer');
const fileInput = document.getElementById('fileToScan');
const btnSend = document.getElementById("sendScan");

dropArea.addEventListener('dragover', (e) => {
  e.preventDefault();
  dropArea.classList.add('hovered');
});

dropArea.addEventListener('mouseleave', (e) => {
  e.preventDefault();
  dropArea.classList.remove('hovered');
});

dropArea.addEventListener('drop', (e) => {
  e.preventDefault();

  const files = e.dataTransfer.files;

  if (files.length > 0) {
    fileInput.files = files;
    fileInput.dispatchEvent(new Event('change'));
  }
});

document.addEventListener("keyup", function (event) {
  if (event.key === "Enter") {
    btnSend.click();
  }
});

document.getElementById("expectedExtension").addEventListener("keyup", function (event) {
  if (event.key === "Enter") {
    btnSend.click();
  }
});

fileInput.addEventListener("change", function () {
  const maximumSize = document.getElementById("maximumSize");
  const scanResultBox = document.getElementById("scanResultBox");
  const expectedExtension = document.getElementById('expectedExtension');

  if (this.files.length > 0) {
    const file = this.files[0];
    if (file.size > 15 * 1024 * 1024) {
      maximumSize.style.display = "block";
      this.value = "";
      document.getElementById('fileNameDisplay').textContent = null;
    } else {
      document.getElementById('fileNameDisplay').textContent = this.value.split('\\').pop();
      expectedExtension.focus();
      expectedExtension.value = this.value.split('.').pop();
      expectedExtension.dispatchEvent(new Event('change'));
      maximumSize.style.display = "none";
    }

    if (scanResultBox != null) {
      scanResultBox.style.display = "none"
    }
  }
});

btnSend.addEventListener("click", function() {
  if (document.getElementById('sendButton').style.display != 'none')
  {
    document.getElementById('scanForm').submit();
  }
  document.getElementById('sendButton').style.display = 'none';
  document.getElementById('loader').style.display = 'block';
});
