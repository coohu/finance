package usecase

import (
	"context"
	"finance-server/internal/domain"
	"fmt"
	"time"
)

type VoucherUsecase interface {
	Save(ctx context.Context, tid int64, voucher *domain.Voucher) (int64, error)
	Get(ctx context.Context, tid int64, id int64) (*domain.Voucher, error)
	Delete(ctx context.Context, tid int64, id int64) error
	List(ctx context.Context, tid int64, filter map[string]interface{}) ([]*domain.Voucher, error)
	Check(ctx context.Context, tid int64, id int64) error
}

type voucherUsecase struct {
	repo        domain.VoucherRepository
	subjectRepo domain.AccountSubjectRepository
}

func NewVoucherUsecase(repo domain.VoucherRepository, subjectRepo domain.AccountSubjectRepository) VoucherUsecase {
	return &voucherUsecase{repo: repo, subjectRepo: subjectRepo}
}

func (u *voucherUsecase) Save(ctx context.Context, tid int64, voucher *domain.Voucher) (int64, error) {
	if len(voucher.Entries) == 0 {
		return 0, fmt.Errorf("voucher must have entries")
	}

	var totalAmount float64
	for _, entry := range voucher.Entries {
		totalAmount += float64(entry.Direction) * entry.Amount
	}

	if totalAmount != 0 {
		return 0, fmt.Errorf("amount imbalance")
	}

	if voucher.Header.ID == 0 {
		return u.repo.Create(ctx, tid, voucher)
	}
	err := u.repo.Update(ctx, tid, voucher)
	return voucher.Header.ID, err
}

func (u *voucherUsecase) Get(ctx context.Context, tid int64, id int64) (*domain.Voucher, error) {
	return u.repo.GetByID(ctx, tid, id)
}

func (u *voucherUsecase) Delete(ctx context.Context, tid int64, id int64) error {
	v, err := u.repo.GetByID(ctx, tid, id)
	if err != nil {
		return err
	}
	if v.Header.Status > 0 {
		return fmt.Errorf("incorrect state: cannot delete checked voucher")
	}
	return u.repo.Delete(ctx, tid, id)
}

func (u *voucherUsecase) List(ctx context.Context, tid int64, filter map[string]interface{}) ([]*domain.Voucher, error) {
	return u.repo.List(ctx, tid, filter)
}

func (u *voucherUsecase) Check(ctx context.Context, tid int64, id int64) error {
	v, err := u.repo.GetByID(ctx, tid, id)
	if err != nil {
		return err
	}
	v.Header.Status = 1
	now := time.Now()
	v.Header.CheckTime = &now
	return u.repo.Update(ctx, tid, v)
}
