package repository

import (
	"context"
	"finance-server/internal/domain"
	"gorm.io/gorm"
)

type voucherRepository struct {
	mgr *SQLiteManager
}

func NewVoucherRepository(mgr *SQLiteManager) domain.VoucherRepository {
	return &voucherRepository{mgr: mgr}
}

func (r *voucherRepository) GetByID(ctx context.Context, tid int64, id int64) (*domain.Voucher, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}

	var header domain.VoucherHeader
	if err := db.WithContext(ctx).First(&header, "_id = ?", id).Error; err != nil {
		return nil, err
	}

	var entries []domain.VoucherEntry
	if err := db.WithContext(ctx).Find(&entries, "_id = ?", id).Error; err != nil {
		return nil, err
	}

	return &domain.Voucher{
		Header:  header,
		Entries: entries,
	}, nil
}

func (r *voucherRepository) Create(ctx context.Context, tid int64, voucher *domain.Voucher) (int64, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return 0, err
	}

	err = db.WithContext(ctx).Transaction(func(tx *gorm.DB) error {
		if err := tx.Create(&voucher.Header).Error; err != nil {
			return err
		}
		for i := range voucher.Entries {
			voucher.Entries[i].ID = voucher.Header.ID
		}
		if err := tx.Create(&voucher.Entries).Error; err != nil {
			return err
		}
		return nil
	})

	return voucher.Header.ID, err
}

func (r *voucherRepository) Update(ctx context.Context, tid int64, voucher *domain.Voucher) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}

	return db.WithContext(ctx).Transaction(func(tx *gorm.DB) error {
		if err := tx.Save(&voucher.Header).Error; err != nil {
			return err
		}
		if err := tx.Where("_id = ?", voucher.Header.ID).Delete(&domain.VoucherEntry{}).Error; err != nil {
			return err
		}
		for i := range voucher.Entries {
			voucher.Entries[i].ID = voucher.Header.ID
		}
		if err := tx.Create(&voucher.Entries).Error; err != nil {
			return err
		}
		return nil
	})
}

func (r *voucherRepository) Delete(ctx context.Context, tid int64, id int64) error {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return err
	}

	return db.WithContext(ctx).Transaction(func(tx *gorm.DB) error {
		if err := tx.Where("_id = ?", id).Delete(&domain.VoucherHeader{}).Error; err != nil {
			return err
		}
		if err := tx.Where("_id = ?", id).Delete(&domain.VoucherEntry{}).Error; err != nil {
			return err
		}
		return nil
	})
}

func (r *voucherRepository) List(ctx context.Context, tid int64, filter map[string]interface{}) ([]*domain.Voucher, error) {
	db, err := r.mgr.GetDB(tid)
	if err != nil {
		return nil, err
	}

	var headers []domain.VoucherHeader
	if err := db.WithContext(ctx).Find(&headers).Error; err != nil {
		return nil, err
	}

	if len(headers) == 0 {
		return []*domain.Voucher{}, nil
	}

	var ids []int64
	for _, h := range headers {
		ids = append(ids, h.ID)
	}

	var allEntries []domain.VoucherEntry
	if err := db.WithContext(ctx).Where("_id IN ?", ids).Find(&allEntries).Error; err != nil {
		return nil, err
	}

	entryMap := make(map[int64][]domain.VoucherEntry)
	for _, e := range allEntries {
		entryMap[e.ID] = append(entryMap[e.ID], e)
	}

	vouchers := make([]*domain.Voucher, 0, len(headers))
	for _, h := range headers {
		vouchers = append(vouchers, &domain.Voucher{
			Header:  h,
			Entries: entryMap[h.ID],
		})
	}

	return vouchers, nil
}
