package repository

import (
	"finance-server/internal/domain"
	"gorm.io/gorm"
)

func AutoMigrate(db *gorm.DB) error {
	return db.AutoMigrate(
		&domain.User{},
		&domain.VoucherHeader{},
		&domain.VoucherEntry{},
		&domain.AccountSubject{},
		&domain.AccountCtl{},
	)
}
