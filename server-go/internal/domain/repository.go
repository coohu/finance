package domain

import (
	"context"
)

type VoucherRepository interface {
	GetByID(ctx context.Context, tid int64, id int64) (*Voucher, error)
	Create(ctx context.Context, tid int64, voucher *Voucher) (int64, error)
	Update(ctx context.Context, tid int64, voucher *Voucher) error
	Delete(ctx context.Context, tid int64, id int64) error
	List(ctx context.Context, tid int64, filter map[string]interface{}) ([]*Voucher, error)
}

type UserRepository interface {
	GetByUserName(ctx context.Context, tid int64, userName string) (*User, error)
	GetByID(ctx context.Context, tid int64, id int64) (*User, error)
	Create(ctx context.Context, tid int64, user *User) error
	Update(ctx context.Context, tid int64, user *User) error
	List(ctx context.Context, tid int64) ([]*User, error)
}

type AccountSubjectRepository interface {
	List(ctx context.Context, tid int64, status int) ([]*AccountSubject, error)
	GetByID(ctx context.Context, tid int64, id int64) (*AccountSubject, error)
	GetByNo(ctx context.Context, tid int64, no string) (*AccountSubject, error)
	Save(ctx context.Context, tid int64, aso *AccountSubject) error
	Delete(ctx context.Context, tid int64, id int64) error
}
