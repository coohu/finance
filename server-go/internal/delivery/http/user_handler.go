package http

import (
	"finance-server/internal/domain"
	"finance-server/internal/usecase"
	"net/http"

	"github.com/gin-gonic/gin"
)

type UserHandler struct {
	usecase usecase.UserUsecase
}

func NewUserHandler(u usecase.UserUsecase) *UserHandler {
	return &UserHandler{usecase: u}
}

type LoginRequest struct {
	UserName string `json:"UserName"`
	PassWord string `json:"PassWord"`
	Tid      int64  `json:"Tid"`
}

func (h *UserHandler) Login(c *gin.Context) {
	var req struct {
		Content LoginRequest `json:"Content"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.NullRequest, ErrMsg: err.Error()})
		return
	}

	user, token, err := h.usecase.Login(c.Request.Context(), req.Content.Tid, req.Content.UserName, req.Content.PassWord)
	if err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.AuthenticationError, ErrMsg: err.Error()})
		return
	}

	c.JSON(http.StatusOK, gin.H{
		"Result":   domain.Success,
		"UserId":   user.UserID,
		"UserName": user.UserName,
		"Token":    token,
	})
}
