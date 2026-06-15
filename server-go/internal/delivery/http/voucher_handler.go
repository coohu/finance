package http

import (
	"finance-server/internal/domain"
	"finance-server/internal/usecase"
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
)

type VoucherHandler struct {
	usecase usecase.VoucherUsecase
}

func NewVoucherHandler(u usecase.VoucherUsecase) *VoucherHandler {
	return &VoucherHandler{usecase: u}
}

type VoucherSaveRequest struct {
	Content domain.Voucher `json:"Content"`
}

func (h *VoucherHandler) Save(c *gin.Context) {
	var req VoucherSaveRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.NullRequest, ErrMsg: err.Error()})
		return
	}

	tid := c.GetInt64("Tid")
	id, err := h.usecase.Save(c.Request.Context(), tid, &req.Content)
	if err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.SystemError, ErrMsg: err.Error()})
		return
	}

	c.JSON(http.StatusOK, domain.IdResponse{ID: id, FinanceResponse: domain.FinanceResponse{Result: domain.Success}})
}

func (h *VoucherHandler) List(c *gin.Context) {
	tid := c.GetInt64("Tid")
	list, err := h.usecase.List(c.Request.Context(), tid, nil)
	if err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.SystemError, ErrMsg: err.Error()})
		return
	}
	c.JSON(http.StatusOK, gin.H{"Content": list, "Result": domain.Success})
}

func (h *VoucherHandler) Delete(c *gin.Context) {
	idStr := c.Query("id")
	id, _ := strconv.ParseInt(idStr, 10, 64)
	tid := c.GetInt64("Tid")
	err := h.usecase.Delete(c.Request.Context(), tid, id)
	if err != nil {
		c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.SystemError, ErrMsg: err.Error()})
		return
	}
	c.JSON(http.StatusOK, domain.FinanceResponse{Result: domain.Success})
}
