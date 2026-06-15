package domain

import "time"

type VoucherHeader struct {
	ID           int64      `gorm:"primaryKey;column:_id" json:"id"`
	Word         string     `gorm:"column:_word" json:"word"`
	No           int64      `gorm:"column:_no" json:"no"`
	SerialNo     int64      `gorm:"column:_serialNo" json:"serialNo"`
	Note         string     `gorm:"column:_note" json:"note"`
	Reference    string     `gorm:"column:_reference" json:"reference"`
	Year         int        `gorm:"column:_year" json:"year"`
	Period       int        `gorm:"column:_period" json:"period"`
	BusinessDate time.Time  `gorm:"column:_businessDate" json:"businessDate"`
	Date         time.Time  `gorm:"column:_date" json:"date"`
	CreateTime   time.Time  `gorm:"column:_creatTime" json:"creatTime"`
	Creator      int64      `gorm:"column:_creater" json:"creater"`
	Cashier      string     `gorm:"column:_cashier" json:"cashier"`
	Agent        string     `gorm:"column:_agent" json:"agent"`
	Poster       int64      `gorm:"column:_poster" json:"poster"`
	Checker      int64      `gorm:"column:_checker" json:"checker"`
	CheckTime    *time.Time `gorm:"column:_checkTime" json:"checkTime"`
	PostTime     *time.Time `gorm:"column:_postTime" json:"postTime"`
	Status       int        `gorm:"column:_status" json:"status"`
}

func (VoucherHeader) TableName() string {
	return "_VoucherHeader"
}

type VoucherEntry struct {
	ID               int64   `gorm:"column:_id" json:"id"`
	Index            int     `gorm:"column:_index" json:"index"`
	AccountSubjectID int64   `gorm:"column:_accountSubjectId" json:"accountSubjectId"`
	AccountSubjectNo string  `gorm:"-" json:"accountSubjectNo"`
	Explanation      string  `gorm:"column:_explanation" json:"explanation"`
	Amount           float64 `gorm:"column:_amount" json:"amount"`
	Direction        int     `gorm:"column:_direction" json:"direction"`
	UniqueKey        string  `gorm:"column:_uniqueKey" json:"uniqueKey"`
}

func (VoucherEntry) TableName() string {
	return "_VoucherEntry"
}

type Voucher struct {
	Header      VoucherHeader                     `json:"header"`
	Entries     []VoucherEntry                    `json:"entries"`
	UdefEntries map[string]map[string]interface{} `json:"udefenties"`
}
