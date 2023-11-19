import AsyncStorage from '@react-native-async-storage/async-storage';

export const getData = async key => {
  try {
    const value = await AsyncStorage.getItem(key);
    if (value !== null) {
      console.log(value)
      return value;
    } else {
      return null;
    }
  } catch (e) {
    console.error(e);
    return null;
  }
};
export const storeData = async (key, value) => {
  try {
    await AsyncStorage.setItem(key, value);
  } catch (e) {
    console.error(e);
  }
};
