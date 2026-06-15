package auth

import (
	"encoding/base64"
	"fmt"
	"strings"
	"time"
)

func CreateToken(userID int64, userName string, tid int64, now time.Time) string {
	raw := fmt.Sprintf("%d|%s|%d|%d", userID, userName, tid, now.Unix())
	return base64.StdEncoding.EncodeToString([]byte(raw))
}

func ParseToken(token string) (userID int64, userName string, tid int64, err error) {
	data, err := base64.StdEncoding.DecodeString(token)
	if err != nil {
		return 0, "", 0, err
	}
	parts := strings.Split(string(data), "|")
	if len(parts) < 3 {
		return 0, "", 0, fmt.Errorf("invalid token")
	}
	fmt.Sscanf(parts[0], "%d", &userID)
	userName = parts[1]
	fmt.Sscanf(parts[2], "%d", &tid)
	return userID, userName, tid, nil
}
