package usecase

import (
	"context"
	"finance-server/internal/domain"
	"finance-server/internal/infrastructure/auth"
	"fmt"
	"time"
)

type UserUsecase interface {
	Login(ctx context.Context, tid int64, userName, password string) (*domain.User, string, error)
	AddUser(ctx context.Context, tid int64, userName, password string) error
	List(ctx context.Context, tid int64) ([]*domain.User, error)
}

type userUsecase struct {
	repo domain.UserRepository
}

func NewUserUsecase(repo domain.UserRepository) UserUsecase {
	return &userUsecase{repo: repo}
}

func (u *userUsecase) Login(ctx context.Context, tid int64, userName, password string) (*domain.User, string, error) {
	user, err := u.repo.GetByUserName(ctx, tid, userName)
	if err != nil {
		return nil, "", err
	}

	decrypted, _ := auth.DecryptPassword(password)
	hashed := auth.MD5Encode(decrypted)

	if user.PassWord != hashed {
		return nil, "", fmt.Errorf("invalid password")
	}

	token := auth.CreateToken(user.UserID, user.UserName, tid, time.Now())
	return user, token, nil
}

func (u *userUsecase) AddUser(ctx context.Context, tid int64, userName, password string) error {
	decrypted, _ := auth.DecryptPassword(password)
	hashed := auth.MD5Encode(decrypted)
	user := &domain.User{
		UserName: userName,
		PassWord: hashed,
	}
	return u.repo.Create(ctx, tid, user)
}

func (u *userUsecase) List(ctx context.Context, tid int64) ([]*domain.User, error) {
	return u.repo.List(ctx, tid)
}
