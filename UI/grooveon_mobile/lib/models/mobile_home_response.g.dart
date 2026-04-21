// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mobile_home_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MobileHomeResponse _$MobileHomeResponseFromJson(Map<String, dynamic> json) =>
    MobileHomeResponse(
      songOfTheDay: json['songOfTheDay'] == null
          ? null
          : MusicStatItemResponse.fromJson(
              json['songOfTheDay'] as Map<String, dynamic>,
            ),
      topTracks:
          (json['topTracks'] as List<dynamic>?)
              ?.map(
                (e) =>
                    MusicStatItemResponse.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
      topArtists:
          (json['topArtists'] as List<dynamic>?)
              ?.map(
                (e) =>
                    MusicStatItemResponse.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
    );

Map<String, dynamic> _$MobileHomeResponseToJson(MobileHomeResponse instance) =>
    <String, dynamic>{
      'songOfTheDay': instance.songOfTheDay,
      'topTracks': instance.topTracks,
      'topArtists': instance.topArtists,
    };
