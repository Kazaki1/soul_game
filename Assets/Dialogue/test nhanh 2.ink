// Biến theo dõi nhánh đã chọn
VAR da_chon_C = false
VAR da_chon_D = false
VAR da_chon_E = false

// đoạn mở đầu để Ink Editor bắt đầu
"Bạn bắt đầu nói chuyện với NPC..."
-> npc_start

=== npc_start ===
{ 
    - da_chon_C && !da_chon_D && !da_chon_E: -> tiep_tuc_C
    - da_chon_D && !da_chon_C && !da_chon_E: -> tiep_tuc_D  
    - da_chon_E && !da_chon_C && !da_chon_D: -> tiep_tuc_E
    - else: -> chua_chon_nhanh_nao
}

=== chua_chon_nhanh_nao ===
"Xin chào, ta là NPC A."
"Ngươi muốn hỏi gì?"
+ "Hỏi về nhiệm vụ C"
    -> thoai_C
+ "Hỏi chuyện khác (D)"
    -> thoai_D
+ "Hỏi về bí mật (E)"
    -> thoai_E
-> END

=== thoai_C ===
"Đây là nội dung thoại C."
"Ta giao cho ngươi nhiệm vụ C. Hãy hoàn thành nó."
~ da_chon_C = true
-> END

=== tiep_tuc_C ===
"Ngươi đã nhận nhiệm vụ C rồi. Ta sẽ nói tiếp phần tiếp theo đây..."
"Nhánh C tiếp tục ở đây..."
+ [Tiếp tục nhiệm vụ C]
    "Đây là bước tiếp theo của nhiệm vụ C..."
    -> END
+ [Hỏi thêm]
    "Ngươi còn thắc mắc gì về nhiệm vụ C không?"
    -> END
-> END

=== thoai_D ===
"Đây là nhánh D."
"Chỉ là câu chuyện ngoài lề thôi."
~ da_chon_D = true
-> END

=== tiep_tuc_D ===
"Ngươi đã hỏi chuyện D rồi. Muốn nghe thêm không?"
+ [Kể tiếp chuyện D]
    "Đây là phần tiếp theo của câu chuyện D..."
    -> END
+ [Thôi]
    "Được rồi, lần sau nói tiếp."
    -> END
-> END

=== thoai_E ===
"Đây là nhánh E - bí mật đặc biệt!"
"Chỉ những người đặc biệt mới biết điều này."
~ da_chon_E = true
-> END

=== tiep_tuc_E ===
"Ngươi đã biết bí mật E rồi. Có muốn biết thêm không?"
+ [Biết thêm]
    "Đây là thông tin bí mật tiếp theo..."
    -> END
+ [Đủ rồi]
    "Tốt, giữ bí mật này nhé."
    -> END
-> END