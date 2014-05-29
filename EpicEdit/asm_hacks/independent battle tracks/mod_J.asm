hirom

;16bit XY (preserve Y)
;16bit A

;U:
;818f14 lda $0124
;818f17 asl a
;818f18 tax
;818f19 ldy $8a79,x
;818f1c ldx $0000,y
;818f1f beq $8f2a
;818f21 phy
;818f22 jsr ($0000,x)
;818f25 ply
;818f26 iny
;J:
;818f28 lda $0124
;818f2b asl a
;818f2c tax
;818f2d ldy $8a8d,x
;818f30 ldx $0000,y
;818f33 beq $8f3e
;818f35 phy
;818f36 jsr ($0000,x)
;818f39 ply
;818f3a iny
;E:
;818f2d lda $0124
;818f30 asl a
;818f31 tax
;818f32 ldy $8a92,x
;818f35 ldx $0000,y
;818f38 beq $8f43
;818f3a phy
;818f3b jsr ($0000,x)
;818f3e ply
;818f3f iny

!target_U	= $C18F14
!target_J	= $C18F28
!target_E	= $C18F2D
!ret1_U		= $818F26
!ret2_U		= $818F18
!ret1_J		= $818F3A
!ret2_J		= $818F2C
!ret1_E		= $818F3F
!ret2_E		= $818F31
!table_U	= $8A79
!table_J	= $8A8D
!table_E	= $8A92

!target		= !target_J
!ret1		= !ret1_J
!ret2		= !ret2_J
!table		= !table_J

org !target
	jml BattleHack

org $C80000

!COURSE		= $0124
!BATTLEBASE	= $14

!P1XPOS		= $1018
!P1YPOS		= $101C
!P1ZERO		= $1020
!P2XPOS		= $1118
!P2YPOS		= $111C
!P2ZERO		= $1120

;BC3: $14
;BC4: $15
;BC1: $16
;BC2: $17

BattleTable:
;BC3
	dw $0200,$0278	;P1
	dw $0200,$018A	;P2
;BC4
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2
;BC1
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2
;BC2
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2

BattleHack:
	lda	!COURSE	;current course is a battle course?
	cmp.w	#!BATTLEBASE
	bcc	.normal
.battle
	sbc.w	#!BATTLEBASE
	asl 	#3	;8 bytes per course
	tax
	lda.l	BattleTable+0,x
	sta.w	!P1XPOS
	lda.l	BattleTable+2,x
	sta.w	!P1YPOS
	lda.l	BattleTable+4,x
	sta.w	!P2XPOS
	lda.l	BattleTable+6,x
	sta.w	!P2YPOS
	stz.w	!P1ZERO	;whatever these do, they are zero'd in this code
	stz.w	!P2ZERO
.return
	lda	!COURSE	;original course, without displacement
	asl	#1
	tax
	ldy	!table,x	;original code
	jml	!ret1	;just before the original iny #2
.normal
	asl 	a
	jml	!ret2