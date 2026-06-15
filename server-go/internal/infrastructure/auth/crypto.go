package auth

import (
	"crypto/aes"
	"crypto/cipher"
	"crypto/md5"
	"encoding/base64"
	"encoding/hex"
	"strings"
)

var (
	aesKey = []byte{0XA6, 0X7D, 0XE1, 0X3F, 0X35, 0X0E, 0XE1, 0XA9, 0X83, 0XA5, 0X62, 0XAA, 0X7A, 0XAE, 0X79, 0X98, 0XA7, 0X33, 0X49, 0XFF, 0XE6, 0XAE, 0XBF, 0X8D, 0X8D, 0X20, 0X8A, 0X49, 0X31, 0X3A, 0X12, 0X40}
	aesIV  = []byte{0XF8, 0X8B, 0X01, 0XFB, 0X08, 0X85, 0X9A, 0XA4, 0XBE, 0X45, 0X28, 0X56, 0X03, 0X42, 0XF6, 0X19}
)

func MD5Encode(s string) string {
	h := md5.New()
	h.Write([]byte(s))
	return strings.ToUpper(hex.EncodeToString(h.Sum(nil)))
}

func SignRequest(body string, secret string) string {
	query := secret + body + secret
	return MD5Encode(query)
}

func DecryptPassword(encryptedBase64 string) (string, error) {
	data, err := base64.StdEncoding.DecodeString(encryptedBase64)
	if err != nil {
		return "", err
	}

	block, err := aes.NewCipher(aesKey)
	if err != nil {
		return "", err
	}

	mode := cipher.NewCBCDecrypter(block, aesIV)
	decrypted := make([]byte, len(data))
	mode.CryptBlocks(decrypted, data)

	decrypted = pkcs7Unpadding(decrypted)
	return string(decrypted), nil
}

func pkcs7Unpadding(data []byte) []byte {
	length := len(data)
	if length == 0 {
		return data
	}
	unpadding := int(data[length-1])
	if unpadding > length {
		return data
	}
	return data[:(length - unpadding)]
}
