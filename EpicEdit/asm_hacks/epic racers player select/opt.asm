arch snes.cpu

macro orgl n
    org (({n} & 0x7f0000) >> 1) | ({n} & 0x7fff)
    base {n}
endmacro

macro orgh n
  org {n} & 0x3fffff
  base {n}
endmacro

if 0
	808e01 php                    A:ff01 X:1f28 Y:0060 S:1fe0 D:0000 DB:80 nvmxdIzc V:229 H:1080
	808e02 sep #$30               A:ff01 X:1f28 Y:0060 S:1fdf D:0000 DB:80 nvmxdIzc V:229 H:1100
	808e04 lda #$01               A:ff01 X:0028 Y:0060 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1118
	808e06 sta $4300     [804300] A:ff01 X:0028 Y:0060 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1130
	808e09 lda #$18               A:ff01 X:0028 Y:0060 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1154
	808e0b sta $4301     [804301] A:ff18 X:0028 Y:0060 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1166
	808e0e lda #$80               A:ff18 X:0028 Y:0060 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1190
	808e10 sta $2115     [802115] A:ff80 X:0028 Y:0060 S:1fdf D:0000 DB:80 NvMXdIzc V:229 H:1202
	808e13 ldy #$01               A:ff80 X:0028 Y:0060 S:1fdf D:0000 DB:80 NvMXdIzc V:229 H:1226
	808e15 ldx $4a       [00004a] A:ff80 X:0028 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1238
	808e17 beq $8e4b     [808e4b] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1258
	808e19 lda $0e9a,x   [800efa] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1270
	808e1c sta $2116     [802116] A:ff00 X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIZc V:229 H:1296
	808e1f lda $0e9b,x   [800efb] A:ff00 X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIZc V:229 H:1320
	808e22 sta $2117     [802117] A:ff5b X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:229 H:1346
	808e25 lda $0e9c,x   [800efc] A:ff5b X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:230 H:   6
	808e28 sta $4302     [804302] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H:  32
	808e2b lda $0e9d,x   [800efd] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H:  56
	808e2e sta $4303     [804303] A:ff2f X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:230 H:  82
	808e31 lda $0e9e,x   [800efe] A:ff2f X:0060 Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:230 H: 106
	808e34 sta $4304     [804304] A:ffc0 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 132
	808e37 lda $0e9f,x   [800eff] A:ffc0 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 156
	808e3a sta $4305     [804305] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 182
	808e3d stz $4306     [804306] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 206
	808e40 sty $420b     [80420b] A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 230
	808e43 dex                    A:ff80 X:0060 Y:0001 S:1fdf D:0000 DB:80 NvMXdIzc V:230 H: 254
	808e44 dex                    A:ff80 X:005f Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:230 H:1356
	808e45 dex                    A:ff80 X:005e Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:231 H:   4
	808e46 dex                    A:ff80 X:005d Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:231 H:  16
	808e47 dex                    A:ff80 X:005c Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:231 H:  28
	808e48 dex                    A:ff80 X:005b Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:231 H:  40
	808e49 bne $8e19     [808e19] A:ff80 X:005a Y:0001 S:1fdf D:0000 DB:80 nvMXdIzc V:231 H:  52
	808e4b stz $4a       [00004a] A:ff80 X:0000 Y:0001 S:1fdf D:0000 DB:80 nvMXdIZc V:247 H:1244
	808e4d plp                    A:ff80 X:0000 Y:0001 S:1fdf D:0000 DB:80 nvMXdIZc V:247 H:1264
	808e4e rts                    A:ff80 X:0000 Y:0001 S:1fe0 D:0000 DB:80 nvmxdIzc V:247 H:1290
endif

{orgh $808e01}
Main:
	php
	sep #$31	//carry only needs to be cleared once
	lda #$01 ; sta $4300
	lda #$18 ; sta $4301
	lda #$80 ; sta $2115
	ldy #$01
	ldx $4a ; beq .return
	rep #$20
	lda #$4300 ; tad
.loop:
	lda $0e9a,x ; sta $2116						//808e19 lda $0e9a,x
	lda $0e9c,x ; sta <$4302					//808e25 lda $0e9c,x
	lda $0e9d,x ; sta <$4303
	lda $0e9f,x ; and #$00ff ; sta <$4305		//808e37 lda $0e9f,x
	sty $420b
	txa ; sbc #>6 ; tax
	bne .loop
	lda #$0000 ; tad
.return:
	sep #$20
	stz $4a
	plp
	rts