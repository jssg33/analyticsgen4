#include <iostream>
#include <cstring>
#include <ctime>

class CockyCipher
{
private:
    char data[1550];
    unsigned char key[5];

    void generateKey()
    {
        constexpr int BASE_SHIFT = 161;

        std::time_t now = std::time(nullptr);
        std::tm* utc = std::gmtime(&now);

        int day    = utc->tm_mday;
        int hour   = utc->tm_hour;
        int minute = utc->tm_min;

        // Apply UTC-4 without changing the day.
        hour -= 4;

        if (hour < 0)
            hour += 24;

        // Convert to 12-hour format.
        if (hour > 12)
            hour -= 12;

        if (hour == 0)
            hour = 12;

        int startShift =
            BASE_SHIFT +
            day +
            hour +
            minute;

        for (int i = 0; i < 5; ++i)
        {
            key[i] = static_cast<unsigned char>(
                (startShift - (i * 2)) & 0xFF
            );
        }
    }

public:
    CockyCipher()
    {
        std::memset(data, 0, sizeof(data));
        std::memset(key, 0, sizeof(key));

        generateKey();
    }

    void setText(const char* text)
    {
        std::strncpy(data, text, sizeof(data) - 1);
        data[sizeof(data) - 1] = '\0';
    }

    const char* getText() const
    {
        return data;
    }

    void insertSpacesEvery5()
    {
        char temp[1550] = {0};

        size_t src = 0;
        size_t dst = 0;
        int count = 0;

        while (data[src] != '\0' &&
               dst < sizeof(temp) - 2)
        {
            temp[dst++] = data[src++];
            count++;

            if (count == 5)
            {
                temp[dst++] = ' ';
                count = 0;
            }
        }

        temp[dst] = '\0';

        std::strncpy(data, temp, sizeof(data) - 1);
        data[sizeof(data) - 1] = '\0';
    }

    void encrypt()
    {
        int position = 0;

        for (size_t i = 0; data[i] != '\0'; ++i)
        {
            if (data[i] == ' ')
                continue;

            unsigned char value =
                static_cast<unsigned char>(data[i]);

            value = static_cast<unsigned char>(
                (value + key[position % 5]) & 0xFF
            );

            data[i] = static_cast<char>(value);

            position++;
        }
    }

    void decrypt()
    {
        int position = 0;

        for (size_t i = 0; data[i] != '\0'; ++i)
        {
            if (data[i] == ' ')
                continue;

            unsigned char value =
                static_cast<unsigned char>(data[i]);

            value = static_cast<unsigned char>(
                (value - key[position % 5]) & 0xFF
            );

            data[i] = static_cast<char>(value);

            position++;
        }
    }

    void printKey() const
    {
        std::cout << "Key: ";

        for (int i = 0; i < 5; ++i)
        {
            std::cout << static_cast<int>(key[i]) << ' ';
        }

        std::cout << std::endl;
    }
};

int main()
{
    CockyCipher cipher;

    cipher.setText("NEWSPAPER IS THE ORIGINAL TEXT");

    std::cout << "Original:\n";
    std::cout << cipher.getText() << "\n\n";

    cipher.insertSpacesEvery5();

    std::cout << "Formatted:\n";
    std::cout << cipher.getText() << "\n\n";

    cipher.printKey();

    cipher.encrypt();

    std::cout << "\nEncrypted:\n";
    std::cout << cipher.getText() << "\n\n";

    cipher.decrypt();

    std::cout << "Decrypted:\n";
    std::cout << cipher.getText() << "\n";

    return 0;
}