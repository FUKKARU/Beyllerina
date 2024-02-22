# データを読み込んで、1文字ごとにリストに格納する.
read_words = ""
f = open("full.txt", "r", encoding="utf-8")
for e in f: read_words = e # 1回のみ繰り返される
f.close()

read_word_list = []
for w in read_words:
    read_word_list.append(w)

# 文字数の4分の1を、切り捨てて計算（w4_listで調整している）.
each_word_list_len = len(read_word_list) // 4

w1_list = []
for i in range(0, each_word_list_len):
    w1_list.append(read_word_list[i])
w1 = "".join(w1_list)

w2_list = []
for i in range(each_word_list_len, 2 * each_word_list_len):
    w2_list.append(read_word_list[i])
w2 = "".join(w2_list)

w3_list = []
for i in range(2 * each_word_list_len, 3 * each_word_list_len):
    w3_list.append(read_word_list[i])
w3 = "".join(w3_list)

w4_list = []
for i in range(3 * each_word_list_len, len(read_word_list)):
    w4_list.append(read_word_list[i])
w4 = "".join(w4_list)

# 4分割した文字を、それぞれのファイルに書き出す.
f = open("1.txt", "w", encoding = "utf-8")
f.write(w1)
f.close()

f = open("2.txt", "w", encoding = "utf-8")
f.write(w2)
f.close()

f = open("3.txt", "w", encoding = "utf-8")
f.write(w3)
f.close()

f = open("4.txt", "w", encoding = "utf-8")
f.write(w4)
f.close()

# アルゴリズムが正しいか、リストの長さの関係から判断する.
if len(w1) + len(w2) + len(w3) + len(w4) == len(read_words): print("成功")
else: print("失敗")
