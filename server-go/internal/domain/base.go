package domain

type FinanceResponse struct {
	Result   int    `json:"Result"`
	ErrMsg   string `json:"ErrMsg"`
	Solution string `json:"Solution"`
}

type IdResponse struct {
	FinanceResponse
	ID int64 `json:"id"`
}

type FinanceRequest struct {
	Token   string      `json:"Token"`
	Content interface{} `json:"Content"`
}

const (
	Success             = 0
	NullDtl             = 1000
	FileNotExist        = 1001
	RecordNotExist      = 1002
	RecordExist         = 1003
	ImperfectData       = 1004
	ServiceTimeout      = 1005
	NullRequest         = 1006
	IncorrectState      = 1007
	AmountImbalance     = 1008
	LinkedData          = 1009
	NotSupport          = 3000
	SystemError         = 3001
	AuthenticationError = 3002
)
