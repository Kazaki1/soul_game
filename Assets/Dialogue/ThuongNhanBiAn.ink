// NPC B - Thương nhân bí ẩn
VAR da_mua_vu_khi = false
VAR da_mua_thuoc = false
VAR da_hoi_bi_mat = false

// đoạn mở đầu
"Bạn tiếp cận một thương nhân bí ẩn với chiếc áo choàng rách rưới..."
-> npc_start

=== npc_start ===
{
    - da_mua_vu_khi && !da_mua_thuoc && !da_hoi_bi_mat: -> tiep_tuc_vu_khi
    - da_mua_thuoc && !da_mua_vu_khi && !da_hoi_bi_mat: -> tiep_tuc_thuoc
    - da_hoi_bi_mat && !da_mua_vu_khi && !da_mua_thuoc: -> tiep_tuc_bi_mat
    - else: -> menu_thuong_nhan
}

=== menu_thuong_nhan ===
"Chào thương nhân, ngươi bán gì vậy?"
"Hừm... Ta có vài món hàng đặc biệt đây."
+ ["Ta muốn mua vũ khí"]
    -> mua_vu_khi
+ ["Ta cần thuốc hồi phục"]
    -> mua_thuoc
+ ["Ngươi biết gì về bí mật của thị trấn?"]
    -> hoi_bi_mat
-> END

=== mua_vu_khi ===
"Đây là thanh kiếm 'Rồng Lửa' - vũ khí hiếm có!"
"500 vàng. Ngươi có đủ không?"
+ ["Mua (500 vàng)"]
    -> mua_vu_khi_xac_nhan
+ ["Không mua"]
    "Hừ, kẻ nghèo kiết xác. Lần sau có tiền quay lại."
    -> END
-> END

=== mua_vu_khi_xac_nhan ===
* ["Đủ vàng"]
    "Khá lắm! Đây là kiếm của ngươi."
    ~ da_mua_vu_khi = true
    "Sức mạnh +50. Cẩn thận khi dùng nhé!"
    -> END
* ["Không đủ vàng"]
    "Hừ, kẻ nghèo kiết xác. Lần sau có tiền quay lại."
    -> END

=== tiep_tuc_vu_khi ===
"Chào chủ nhân của 'Rồng Lửa'!"
"Ngươi đã dùng kiếm chưa? Muốn nâng cấp không?"
+ ["Nâng cấp kiếm (1000 vàng)"]
    "Kiếm giờ thành 'Rồng Lửa Vương'!"
    "Sức mạnh +100!"
    -> END
+ ["Bán lại kiếm"]
    "Ta mua lại 300 vàng thôi. Deal?"
    ~ da_mua_vu_khi = false
    -> END
-> END

=== mua_thuoc ===
"Thuốc hồi phục 'Phượng Hoàng' - hồi 100 HP!"
"Chỉ 200 vàng một lọ."
+ ["Mua 1 lọ"]
    ~ da_mua_thuoc = true
    "Tốt! Giữ cẩn thận, chỉ có 3 lọ trên đời này."
    -> END
+ ["Mua 3 lọ (500 vàng)"]
    ~ da_mua_thuoc = true
    "Khá khôn ngoan! Đây là cả kho báu đấy!"
    -> END
-> END

=== tiep_tuc_thuoc ===
"Thuốc 'Phượng Hoàng' của ngươi còn không?"
+ ["Còn 1 lọ"]
    -> tiep_tuc_thuoc_con
+ ["Hết rồi"]
    "Ngươi dùng hết rồi à? Tiếc thật..."
    "Ta sẽ tìm thêm, quay lại sau nhé."
    -> END
-> END

=== tiep_tuc_thuoc_con ===
"Cẩn thận dùng. Muốn mua thêm không?"
* ["Mua thêm"]
    "Đây là lọ cuối cùng. 300 vàng!"
    -> END
* ["Không mua thêm"]
    -> END

=== hoi_bi_mat ===
"Ngươi gan lắm mới dám hỏi..."
"Thị trấn này có một hang động bí mật ở phía Bắc."
"Chỉ mở vào đêm trăng tròn."
~ da_hoi_bi_mat = true
+ ["Cảm ơn thông tin"]
    "Đi đi, nhưng đừng nói ai biết ta đã nói!"
    -> END
-> END

=== tiep_tuc_bi_mat ===
"Ngươi đã vào hang động chưa?"
"Hang đó chứa kho báu cổ xưa..."
+ ["Chưa đi"]
    "Đi ngay đêm nay đi! Trăng tròn rồi đấy!"
    -> END
+ ["Đã vào rồi"]
    -> tiep_tuc_bi_mat_da_vao
-> END

=== tiep_tuc_bi_mat_da_vao ===
* ["Tìm thấy kho báu"]
    "Tuyệt vời! Giờ ngươi giàu to rồi!"
    -> END
* ["Bị quái vật tấn công"]
    "May mà thoát được! Lần sau cẩn thận hơn."
    -> END