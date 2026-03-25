// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'music_overview_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MusicOverviewResponse _$MusicOverviewResponseFromJson(
  Map<String, dynamic> json,
) => MusicOverviewResponse(
  mode: json['mode'] as String,
  year: (json['year'] as num).toInt(),
  month: (json['month'] as num?)?.toInt(),
  mostPlayedSongs: (json['mostPlayedSongs'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  leastPlayedSongs: (json['leastPlayedSongs'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  mostPlayedAlbums: (json['mostPlayedAlbums'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  leastPlayedAlbums: (json['leastPlayedAlbums'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  mostPlayedArtists: (json['mostPlayedArtists'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  leastPlayedArtists: (json['leastPlayedArtists'] as List<dynamic>)
      .map((e) => MusicStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  trendingGenres: (json['trendingGenres'] as List<dynamic>)
      .map((e) => GenreStatItemResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$MusicOverviewResponseToJson(
  MusicOverviewResponse instance,
) => <String, dynamic>{
  'mode': instance.mode,
  'year': instance.year,
  'month': instance.month,
  'mostPlayedSongs': instance.mostPlayedSongs.map((e) => e.toJson()).toList(),
  'leastPlayedSongs': instance.leastPlayedSongs.map((e) => e.toJson()).toList(),
  'mostPlayedAlbums': instance.mostPlayedAlbums.map((e) => e.toJson()).toList(),
  'leastPlayedAlbums': instance.leastPlayedAlbums
      .map((e) => e.toJson())
      .toList(),
  'mostPlayedArtists': instance.mostPlayedArtists
      .map((e) => e.toJson())
      .toList(),
  'leastPlayedArtists': instance.leastPlayedArtists
      .map((e) => e.toJson())
      .toList(),
  'trendingGenres': instance.trendingGenres.map((e) => e.toJson()).toList(),
};
