function enableEdit(id) {
    document.getElementById('view-url-' + id).style.display = 'none';
    document.getElementById('edit-url-' + id).style.display = 'block';
}

function cancelEdit(id) {
    document.getElementById('view-url-' + id).style.display = 'block';
    document.getElementById('edit-url-' + id).style.display = 'none';
}

async function saveEdit(id) {
    var newUrl = document.getElementById('edit-url-input-' + id).value;

    // Get the antiforgery token
    const token = '@Antiforgery.GetAndStoreTokens(HttpContextAccessor.HttpContext).RequestToken';

    // Send AJAX request to the server to save the new URL
    const response = await fetch('/UrlShortener/EditUrl', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            id: id,
            newUrl: newUrl
        })
    });

    if (response.ok) {
        // Update the view
        document.getElementById('view-url-' + id).innerText = newUrl;
        cancelEdit(id);
    } else {
        alert("Failed to save the new URL.");
    }
}

async function getQrCode(shortenedKey) {

    // Send request to get the QR code
    const response = await fetch(`/api/qrcode/GetQrCode?shortenedKey=${encodeURIComponent(shortenedKey)}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        }
    });


    if (response.ok) {
        const data = await response.text();
        return data;
    } else {
        alert("Failed to get the QR code");
    }
}

async function displayQrCode(shortenedKey) {
    const qrCodeBase64 = await getQrCode(shortenedKey);
    const modal = document.getElementById('qrModal');
    const qrCodeContainer = document.getElementById('qrCodeContainer');

    qrCodeContainer.innerHTML = `<img src="data:image/png;base64,${qrCodeBase64}" alt="QR Code" style="max-width: 100%; height: auto;"/>`;

    modal.style.display = 'flex';
}

function closeModal() {
    const modal = document.getElementById('qrModal');

    modal.style.display = 'none';
}

function openQrCodeWindow(qrCodeBase64) {
    const qrWindow = window.open("", "QR Code", "width=300,height=300");

    if (qrWindow) {
        // Write the QR code image to the new window
        qrWindow.document.write(`<html><head><title>QR Code</title></head><body><img src="data:image/png;base64,${qrCodeBase64}" alt="QR Code"/><br><button onclick="window.close()">Close</button></body></html>`);
        qrWindow.document.close();
    }
}

async function deleteUrl(id) {
    if (!confirm("Are you sure?")) return;
    const urlRow = document.getElementById("url-" + id);

    const response = await fetch('/UrlShortener/DeleteUrl', {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            id: id,
        })
    });

    if (response.ok) {
        urlRow.remove();
    }
}