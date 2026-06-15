package http

import (
	"finance-server/internal/infrastructure/auth"

	"github.com/gin-gonic/gin"
)

func AuthMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		token := c.Query("Token")
		if token != "" {
			uid, name, tid, err := auth.ParseToken(token)
			if err == nil {
				c.Set("UserID", uid)
				c.Set("UserName", name)
				c.Set("Tid", tid)
			}
		}
		c.Next()
	}
}

func SignatureMiddleware(secret string) gin.HandlerFunc {
	return func(c *gin.Context) {
		c.Next()
	}
}
