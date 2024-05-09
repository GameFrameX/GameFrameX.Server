using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSocket.ClientEngine
{
    // Token: 0x02000002 RID: 2
    public class ASCIIEncoding : Encoding
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static ASCIIEncoding()
        {
            ASCIIEncoding.m_Instance = new ASCIIEncoding();
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000002 RID: 2 RVA: 0x0000257F File Offset: 0x0000077F
        public static ASCIIEncoding Instance
        {
            get { return ASCIIEncoding.m_Instance; }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002586 File Offset: 0x00000786
        public override string WebName
        {
            get { return "us-ascii"; }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x0000258D File Offset: 0x0000078D
        public override int GetHashCode()
        {
            return this.WebName.GetHashCode();
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000005 RID: 5 RVA: 0x0000259A File Offset: 0x0000079A
        // (set) Token: 0x06000006 RID: 6 RVA: 0x000025A4 File Offset: 0x000007A4
        public char? FallbackCharacter
        {
            get { return this.fallbackCharacter; }
            set
            {
                this.fallbackCharacter = value;
                if (value != null && !ASCIIEncoding.charToByte.ContainsKey(value.Value))
                {
                    throw new EncoderFallbackException(string.Format("Cannot use the character [{0}] (int value {1}) as fallback value - the fallback character itself is not supported by the encoding.", value.Value, (int)value.Value));
                }

                this.FallbackByte = ((value != null) ? new byte?(ASCIIEncoding.charToByte[value.Value]) : null);
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000007 RID: 7 RVA: 0x0000262C File Offset: 0x0000082C
        // (set) Token: 0x06000008 RID: 8 RVA: 0x00002634 File Offset: 0x00000834
        public byte? FallbackByte { get; private set; }

        // Token: 0x06000009 RID: 9 RVA: 0x0000263D File Offset: 0x0000083D
        public ASCIIEncoding()
        {
            this.FallbackCharacter = new char?('?');
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002654 File Offset: 0x00000854
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (this.FallbackByte == null)
            {
                return this.GetBytesWithoutFallback(chars, charIndex, charCount, bytes, byteIndex);
            }

            return this.GetBytesWithFallBack(chars, charIndex, charCount, bytes, byteIndex);
        }

        // Token: 0x0600000B RID: 11 RVA: 0x0000268C File Offset: 0x0000088C
        private int GetBytesWithFallBack(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                char key = chars[i + charIndex];
                byte b;
                bool flag = ASCIIEncoding.charToByte.TryGetValue(key, out b);
                bytes[byteIndex + i] = (flag ? b : this.FallbackByte.Value);
            }

            return charCount;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x000026D8 File Offset: 0x000008D8
        private int GetBytesWithoutFallback(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                char c = chars[i + charIndex];
                byte b;
                if (!ASCIIEncoding.charToByte.TryGetValue(c, out b))
                {
                    throw new EncoderFallbackException(string.Format("The encoding [{0}] cannot encode the character [{1}] (int value {2}). Set the FallbackCharacter property in order to suppress this exception and encode a default character instead.", this.WebName, c, (int)c));
                }

                bytes[byteIndex + i] = b;
            }

            return charCount;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002734 File Offset: 0x00000934
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (this.FallbackCharacter == null)
            {
                return this.GetCharsWithoutFallback(bytes, byteIndex, byteCount, chars, charIndex);
            }

            return this.GetCharsWithFallback(bytes, byteIndex, byteCount, chars, charIndex);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000276C File Offset: 0x0000096C
        private int GetCharsWithFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte b = bytes[i + byteIndex];
                char c = ((int)b >= ASCIIEncoding.byteToChar.Length) ? this.FallbackCharacter.Value : ASCIIEncoding.byteToChar[(int)b];
                chars[charIndex + i] = c;
            }

            return byteCount;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000027B8 File Offset: 0x000009B8
        private int GetCharsWithoutFallback(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                byte b = bytes[i + byteIndex];
                if ((int)b >= ASCIIEncoding.byteToChar.Length)
                {
                    throw new EncoderFallbackException(string.Format("The encoding [{0}] cannot decode byte value [{1}]. Set the FallbackCharacter property in order to suppress this exception and decode the value as a default character instead.", this.WebName, b));
                }

                chars[charIndex + i] = ASCIIEncoding.byteToChar[(int)b];
            }

            return byteCount;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x0000280C File Offset: 0x00000A0C
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x0000280F File Offset: 0x00000A0F
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002812 File Offset: 0x00000A12
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002815 File Offset: 0x00000A15
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000014 RID: 20 RVA: 0x00002818 File Offset: 0x00000A18
        public static int CharacterCount
        {
            get { return ASCIIEncoding.byteToChar.Length; }
        }

        // Token: 0x04000001 RID: 1
        private static ASCIIEncoding m_Instance = null;

        // Token: 0x04000002 RID: 2
        private char? fallbackCharacter;

        // Token: 0x04000004 RID: 4
        private static char[] byteToChar = new char[]
        {
            '\0',
            '\u0001',
            '\u0002',
            '\u0003',
            '\u0004',
            '\u0005',
            '\u0006',
            '\a',
            '\b',
            '\t',
            '\n',
            '\v',
            '\f',
            '\r',
            '\u000e',
            '\u000f',
            '\u0010',
            '\u0011',
            '\u0012',
            '\u0013',
            '\u0014',
            '\u0015',
            '\u0016',
            '\u0017',
            '\u0018',
            '\u0019',
            '\u001a',
            '\u001b',
            '\u001c',
            '\u001d',
            '\u001e',
            '\u001f',
            ' ',
            '!',
            '"',
            '#',
            '$',
            '%',
            '&',
            '\'',
            '(',
            ')',
            '*',
            '+',
            ',',
            '-',
            '.',
            '/',
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            ':',
            ';',
            '<',
            '=',
            '>',
            '?',
            '@',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '[',
            '\\',
            ']',
            '^',
            '_',
            '`',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '{',
            '|',
            '}',
            '~',
            '\u007f'
        };

        // Token: 0x04000005 RID: 5
        private static Dictionary<char, byte> charToByte = new Dictionary<char, byte>
        {
            {
                '\0',
                0
            },
            {
                '\u0001',
                1
            },
            {
                '\u0002',
                2
            },
            {
                '\u0003',
                3
            },
            {
                '\u0004',
                4
            },
            {
                '\u0005',
                5
            },
            {
                '\u0006',
                6
            },
            {
                '\a',
                7
            },
            {
                '\b',
                8
            },
            {
                '\t',
                9
            },
            {
                '\n',
                10
            },
            {
                '\v',
                11
            },
            {
                '\f',
                12
            },
            {
                '\r',
                13
            },
            {
                '\u000e',
                14
            },
            {
                '\u000f',
                15
            },
            {
                '\u0010',
                16
            },
            {
                '\u0011',
                17
            },
            {
                '\u0012',
                18
            },
            {
                '\u0013',
                19
            },
            {
                '\u0014',
                20
            },
            {
                '\u0015',
                21
            },
            {
                '\u0016',
                22
            },
            {
                '\u0017',
                23
            },
            {
                '\u0018',
                24
            },
            {
                '\u0019',
                25
            },
            {
                '\u001a',
                26
            },
            {
                '\u001b',
                27
            },
            {
                '\u001c',
                28
            },
            {
                '\u001d',
                29
            },
            {
                '\u001e',
                30
            },
            {
                '\u001f',
                31
            },
            {
                ' ',
                32
            },
            {
                '!',
                33
            },
            {
                '"',
                34
            },
            {
                '#',
                35
            },
            {
                '$',
                36
            },
            {
                '%',
                37
            },
            {
                '&',
                38
            },
            {
                '\'',
                39
            },
            {
                '(',
                40
            },
            {
                ')',
                41
            },
            {
                '*',
                42
            },
            {
                '+',
                43
            },
            {
                ',',
                44
            },
            {
                '-',
                45
            },
            {
                '.',
                46
            },
            {
                '/',
                47
            },
            {
                '0',
                48
            },
            {
                '1',
                49
            },
            {
                '2',
                50
            },
            {
                '3',
                51
            },
            {
                '4',
                52
            },
            {
                '5',
                53
            },
            {
                '6',
                54
            },
            {
                '7',
                55
            },
            {
                '8',
                56
            },
            {
                '9',
                57
            },
            {
                ':',
                58
            },
            {
                ';',
                59
            },
            {
                '<',
                60
            },
            {
                '=',
                61
            },
            {
                '>',
                62
            },
            {
                '?',
                63
            },
            {
                '@',
                64
            },
            {
                'A',
                65
            },
            {
                'B',
                66
            },
            {
                'C',
                67
            },
            {
                'D',
                68
            },
            {
                'E',
                69
            },
            {
                'F',
                70
            },
            {
                'G',
                71
            },
            {
                'H',
                72
            },
            {
                'I',
                73
            },
            {
                'J',
                74
            },
            {
                'K',
                75
            },
            {
                'L',
                76
            },
            {
                'M',
                77
            },
            {
                'N',
                78
            },
            {
                'O',
                79
            },
            {
                'P',
                80
            },
            {
                'Q',
                81
            },
            {
                'R',
                82
            },
            {
                'S',
                83
            },
            {
                'T',
                84
            },
            {
                'U',
                85
            },
            {
                'V',
                86
            },
            {
                'W',
                87
            },
            {
                'X',
                88
            },
            {
                'Y',
                89
            },
            {
                'Z',
                90
            },
            {
                '[',
                91
            },
            {
                '\\',
                92
            },
            {
                ']',
                93
            },
            {
                '^',
                94
            },
            {
                '_',
                95
            },
            {
                '`',
                96
            },
            {
                'a',
                97
            },
            {
                'b',
                98
            },
            {
                'c',
                99
            },
            {
                'd',
                100
            },
            {
                'e',
                101
            },
            {
                'f',
                102
            },
            {
                'g',
                103
            },
            {
                'h',
                104
            },
            {
                'i',
                105
            },
            {
                'j',
                106
            },
            {
                'k',
                107
            },
            {
                'l',
                108
            },
            {
                'm',
                109
            },
            {
                'n',
                110
            },
            {
                'o',
                111
            },
            {
                'p',
                112
            },
            {
                'q',
                113
            },
            {
                'r',
                114
            },
            {
                's',
                115
            },
            {
                't',
                116
            },
            {
                'u',
                117
            },
            {
                'v',
                118
            },
            {
                'w',
                119
            },
            {
                'x',
                120
            },
            {
                'y',
                121
            },
            {
                'z',
                122
            },
            {
                '{',
                123
            },
            {
                '|',
                124
            },
            {
                '}',
                125
            },
            {
                '~',
                126
            },
            {
                '\u007f',
                127
            }
        };
    }
}