-> start

=== start ===
Câu 1: Xin chào, đây là câu đầu tiên.
Câu 2: Bạn muốn chọn cái nào?
+ [Lựa chọn 1]
    -> after_choice("Bạn đã chọn lựa chọn 1")
+ [Lựa chọn 2]
    -> after_choice("Bạn đã chọn lựa chọn 2")

=== after_choice(msg) ===
{msg}
Câu 3: Đây là câu kết thúc. 
-> END
