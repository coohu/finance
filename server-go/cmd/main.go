package main

import (
	"finance-server/internal/delivery/http"
	"finance-server/internal/repository"
	"finance-server/internal/usecase"
	"log"

	"github.com/gin-gonic/gin"
)

func main() {
	sqliteMgr := repository.NewSQLiteManager("./data")

	mainDB, _ := sqliteMgr.GetDB(-1)
	repository.AutoMigrate(mainDB)

	userRepo := repository.NewUserRepository(sqliteMgr)
	voucherRepo := repository.NewVoucherRepository(sqliteMgr)
	subjectRepo := repository.NewAccountSubjectRepository(sqliteMgr)

	userUC := usecase.NewUserUsecase(userRepo)
	voucherUC := usecase.NewVoucherUsecase(voucherRepo, subjectRepo)

	userHandler := http.NewUserHandler(userUC)
	voucherHandler := http.NewVoucherHandler(voucherUC)

	r := gin.Default()
	r.Use(http.AuthMiddleware())

	r.POST("/user/login", userHandler.Login)

	voucherGroup := r.Group("/voucher")
	{
		voucherGroup.POST("/save", voucherHandler.Save)
		voucherGroup.POST("/list", voucherHandler.List)
		voucherGroup.POST("/delete", voucherHandler.Delete)
	}

	if err := r.Run(":9000"); err != nil {
		log.Fatalf("Failed to run server: %v", err)
	}
}
