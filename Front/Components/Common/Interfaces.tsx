import {Region} from "react-native-maps";

export interface JwtDecodedResult {
    jti: string,
    sub:string,
    role:number
    // Add other properties if jwtDecode returns more than just 'jti'
}
export interface Coordinates{
    latitude: number,
    longitude: number,
    longitudeDelta: number,
    latitudeDelta:number
}
export interface Pin{
    Id: number,
    region: {
        latitude: number,
        longitude: number,
        latitudeDelta: number,
        longitudeDelta: number };
}
export interface RouteInt {
    Id: number;
    RouteName: string;
}
